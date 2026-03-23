using AutoMapper;
using Business.Abstract;
using Business.Dtos.Order;
using Business.Dtos.Payment; // CreateCheckoutSessionDto üçün
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using Entities.Concrete;
using Entities.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderManager(IUnitOfWork unitOfWork, IMapper mapper, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentService = paymentService;
        }

        public async Task<IDataResult<OrderGetDto>> CreateAsync(string userId, OrderCreateDto dto)
        {
            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _unitOfWork.ProductRepository.GetAllAsync(p => productIds.Contains(p.Id));

            if (products.Count != productIds.Count)
                return new ErrorDataResult<OrderGetDto>(null, "Bəzi məhsullar tapılmadı.");

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);

                if (product.StockQuantity < item.Quantity)
                    return new ErrorDataResult<OrderGetDto>(null, $"{product.Name} üçün kifayət qədər stok yoxdur.");

                product.StockQuantity -= item.Quantity;
            }

            var order = new Order
            {
                AppUserId = userId,
                OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                ShippingAddress = dto.ShippingAddress,
                Note = dto.Note,
                OrderStatus = OrderStatus.PendingPayment,
                CreateAt = DateTime.UtcNow,
                OrderItems = dto.Items.Select(item =>
                {
                    var product = products.First(p => p.Id == item.ProductId);
                    return new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        ProductNameSnapshot = product.Name
                    };
                }).ToList()
            };

            order.TotalAmount = order.OrderItems.Sum(x => x.UnitPrice * x.Quantity);

            await _unitOfWork.OrderRepository.AddAsync(order);

            try
            {
                var affected = await _unitOfWork.SaveChangesAsync();

                if (affected <= 0)
                    return new ErrorDataResult<OrderGetDto>(null, "Sifariş bazaya yazıla bilmədi.");

                var paymentResult = await _paymentService.CreateCheckoutSessionAsync(
                    new CreateCheckoutSessionDto { OrderId = order.Id });

                var mappedOrder = _mapper.Map<OrderGetDto>(order);

                if (paymentResult.Success && paymentResult.Data != null)
                {
                    mappedOrder.CheckoutUrl = paymentResult.Data.CheckoutUrl;
                    return new SuccessDataResult<OrderGetDto>(mappedOrder, "Sifariş yaradıldı, ödənişə yönləndirilir.");
                }

                return new ErrorDataResult<OrderGetDto>(mappedOrder, paymentResult.Message ?? "Checkout session yaradıla bilmədi.");
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<OrderGetDto>(null, "Xəta: " + ex.Message);
            }
        }
        public async Task<IDataResult<OrderGetDto>> CreateFromCartAsync(string userId, OrderFromCartDto dto)
        {
            var cart = await _unitOfWork.CartRepository.GetAsync(
                c => c.AppUserId == userId,
                "CartItems", "CartItems.Product"
            );

            if (cart == null || !cart.CartItems.Any())
                return new ErrorDataResult<OrderGetDto>(null, "Səbətiniz boşdur.");

            foreach (var item in cart.CartItems)
            {
                if (item.Product == null)
                    return new ErrorDataResult<OrderGetDto>(null, "Məhsul məlumatı tapılmadı.");

                if (item.Product.StockQuantity < item.Quantity)
                    return new ErrorDataResult<OrderGetDto>(null, $"{item.Product.Name} üçün kifayət qədər stok yoxdur.");
            }

            foreach (var item in cart.CartItems)
            {
                item.Product.StockQuantity -= item.Quantity;
            }

            var order = new Order
            {
                AppUserId = userId,
                OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                ShippingAddress = dto.ShippingAddress,
                Note = dto.Note,
                OrderStatus = OrderStatus.PendingPayment,
                CreateAt = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price,
                    ProductNameSnapshot = ci.Product.Name
                }).ToList()
            };

            order.TotalAmount = order.OrderItems.Sum(x => x.UnitPrice * x.Quantity);

            await _unitOfWork.OrderRepository.AddAsync(order);

            foreach (var item in cart.CartItems)
                _unitOfWork.CartitemRepository.Remove(item);

            var affected = await _unitOfWork.SaveChangesAsync();

            if (affected <= 0)
                return new ErrorDataResult<OrderGetDto>(null, "Sifariş yaradılmadı.");

            var paymentResult = await _paymentService.CreateCheckoutSessionAsync(
                new CreateCheckoutSessionDto { OrderId = order.Id });

            var mappedOrder = _mapper.Map<OrderGetDto>(order);

            if (paymentResult.Success && paymentResult.Data != null)
            {
                mappedOrder.CheckoutUrl = paymentResult.Data.CheckoutUrl;
                return new SuccessDataResult<OrderGetDto>(mappedOrder, "Səbətdən sifariş yaradıldı.");
            }

            return new ErrorDataResult<OrderGetDto>(mappedOrder, paymentResult.Message ?? "Ödəniş linki yaradıla bilmədi.");
        }
        public async Task<IDataResult<OrderGetDto>> GetByIdAsync(int id, string userId)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == id && o.AppUserId == userId,
                "OrderItems", "Payment"
            );

            if (order == null) return new ErrorDataResult<OrderGetDto>(null, "Sifariş tapılmadı.");
            return new SuccessDataResult<OrderGetDto>(_mapper.Map<OrderGetDto>(order));
        }

        public async Task<IDataResult<List<OrderGetDto>>> GetMyOrdersAsync(string userId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                o => o.AppUserId == userId,
                "OrderItems", "Payment"
            );
            return new SuccessDataResult<List<OrderGetDto>>(_mapper.Map<List<OrderGetDto>>(orders));
        }

        public async Task<IDataResult<OrderGetDto>> CancelAsync(int orderId, string userId, OrderCancelDto dto)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == orderId && o.AppUserId == userId,
                "OrderItems", "Payment"
            );

            if (order == null) return new ErrorDataResult<OrderGetDto>(null, "Sifariş tapılmadı.");
            if (order.OrderStatus == OrderStatus.Cancelled) return new ErrorDataResult<OrderGetDto>(null, "Artıq ləğv edilib.");

            // Restock (Stoku geri qaytar)
            foreach (var oi in order.OrderItems)
            {
                var product = await _unitOfWork.ProductRepository.GetAsync(p => p.Id == oi.ProductId);
                if (product != null) product.StockQuantity += oi.Quantity;
            }

            order.OrderStatus = OrderStatus.Cancelled;
            if (order.Payment != null) order.Payment.Status = PaymentStatus.Failed;

            await _unitOfWork.SaveChangesAsync();
            return new SuccessDataResult<OrderGetDto>(_mapper.Map<OrderGetDto>(order), "Sifariş ləğv edildi.");
        }

        public async Task<IDataResult<List<OrderGetDto>>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(null, "OrderItems", "Payment");
            return new SuccessDataResult<List<OrderGetDto>>(_mapper.Map<List<OrderGetDto>>(orders));
        }

        public async Task<IDataResult<OrderGetDto>> UpdateStatusAsync(int orderId, OrderStatusUpdateDto dto)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.Id == orderId, "Payment");
            if (order == null) return new ErrorDataResult<OrderGetDto>(null, "Sifariş tapılmadı.");

            order.OrderStatus = dto.Status;
            await _unitOfWork.SaveChangesAsync();
            return new SuccessDataResult<OrderGetDto>(_mapper.Map<OrderGetDto>(order), "Status yeniləndi.");
        }

        public async Task<IResult> UpdatePaymentStatusAsync(int orderId, bool isSuccess, string providerReference)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == orderId,
                "Payment" // Payment obyektini də yükləyirik
            );

            if (order == null) return new ErrorResult("Sifariş tapılmadı.");

            if (isSuccess)
            {
                // 1. Sifarişin ümumi statusunu "Hazırlanır" və ya "Ödənildi" edirik
                order.OrderStatus = OrderStatus.Preparing; // Və ya sənin Enum-da nədirsə (məs. Paid)

                if (order.Payment != null)
                {
                    // 2. Ödənişin öz statusunu "Success" edirik
                    order.Payment.Status = PaymentStatus.Success;

                    // 3. Stripe-dan gələn referans ID-ni yadda saxlayırıq
                    order.Payment.ProviderReference = providerReference;

                    // 4. Əgər varsa, yenilənmə vaxtını qeyd edirik
                    // order.Payment.UpdateAt = DateTime.UtcNow; 
                }
            }
            else
            {
                // Ödəniş uğursuz olarsa statusu dəyişmirik və ya "Failed" edirik
                if (order.Payment != null)
                {
                    order.Payment.Status = PaymentStatus.Failed;
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return new SuccessResult("Sifariş və ödəniş statusu yeniləndi.");
        }
    }
}