using AutoMapper;
using Business.Abstract;
using Business.Dtos.Payment;
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using Entities.Concrete;
using Entities.Enum;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using WebApi.Settings;

namespace Business.Concrete
{
    public class PaymentManager : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly StripeSetting _stripeSettings;

        public PaymentManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOptions<StripeSetting> stripeOptions)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _stripeSettings = stripeOptions.Value;

            if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
                throw new Exception("Stripe SecretKey tapılmadı.");

            Stripe.StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<IDataResult<CheckoutSessionResponseDto>> CreateCheckoutSessionAsync(CreateCheckoutSessionDto dto)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetAsync(
                    x => x.Id == dto.OrderId && !x.IsDeleted,
                    "OrderItems"
                );

                if (order == null)
                    return new ErrorDataResult<CheckoutSessionResponseDto>(null, "Order tapılmadı");

                if (order.OrderItems == null || !order.OrderItems.Any())
                    return new ErrorDataResult<CheckoutSessionResponseDto>(null, "Order item tapılmadı");

                if (order.OrderStatus == OrderStatus.Paid)
                    return new ErrorDataResult<CheckoutSessionResponseDto>(null, "Bu sifariş artıq ödənib");

                var lineItems = order.OrderItems.Select(item => new SessionLineItemOptions
                {
                    Quantity = item.Quantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "azn",
                        UnitAmount = (long)(item.UnitPrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductNameSnapshot
                        }
                    }
                }).ToList();

                string baseUrl = _stripeSettings.FrontendBaseUrl ?? "http://localhost:5173";
                string orderId = order.Id.ToString();

                var options = new SessionCreateOptions
                {
                    Mode = "payment",
                    SuccessUrl = $"{baseUrl}/payment/success?session_id={{CHECKOUT_SESSION_ID}}&orderId={orderId}",
                    CancelUrl = $"{baseUrl}/payment/cancel?orderId={orderId}",
                    LineItems = lineItems,
                    Metadata = new Dictionary<string, string>
                    {
                        { "orderId", order.Id.ToString() }
                    }
                };

                var sessionService = new SessionService();
                var session = await sessionService.CreateAsync(options);

                var existingPayment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == order.Id);

                if (existingPayment != null)
                {
                    existingPayment.Amount = order.TotalAmount;
                    existingPayment.Currency = "azn";
                    existingPayment.Method = PaymentMethod.Stripe;
                    existingPayment.Status = PaymentStatus.Pending;
                    existingPayment.PaymentDate = DateTime.UtcNow;
                    existingPayment.Provider = "Stripe";
                    existingPayment.ProviderReference = session.Id;
                    existingPayment.FailureReason = null;
                    existingPayment.PaidAt = null;

                    Console.WriteLine($"Mövcud payment update olundu. OrderId: {order.Id}, SessionId: {session.Id}");
                }
                else
                {
                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        Amount = order.TotalAmount,
                        Currency = "azn",
                        Method = PaymentMethod.Stripe,
                        Status = PaymentStatus.Pending,
                        PaymentDate = DateTime.UtcNow,
                        Provider = "Stripe",
                        ProviderReference = session.Id
                    };

                    await _unitOfWork.PaymentRepository.AddAsync(payment);

                    Console.WriteLine($"Yeni payment yaradıldı. OrderId: {order.Id}, SessionId: {session.Id}");
                }

                order.OrderStatus = OrderStatus.PendingPayment;

                await _unitOfWork.SaveChangesAsync();

                var response = new CheckoutSessionResponseDto
                {
                    SessionId = session.Id,
                    CheckoutUrl = session.Url
                };

                return new SuccessDataResult<CheckoutSessionResponseDto>(response, "Checkout session yaradıldı");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CreateCheckoutSessionAsync xətası: {ex.Message}");
                return new ErrorDataResult<CheckoutSessionResponseDto>(null, ex.Message);
            }
        }

        public async Task HandleCheckoutSessionCompletedAsync(Session session)
        {
            try
            {
                if (session == null)
                {
                    Console.WriteLine("Session null gəldi.");
                    return;
                }

                if (session.Metadata == null || !session.Metadata.ContainsKey("orderId"))
                {
                    Console.WriteLine("Webhook gəldi, amma Metadata-da orderId tapılmadı.");
                    return;
                }

                if (!int.TryParse(session.Metadata["orderId"], out int orderId))
                {
                    Console.WriteLine("orderId parse olunmadı.");
                    return;
                }

                Console.WriteLine($"Webhook orderId: {orderId}");

                var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId);

                if (order == null)
                {
                    Console.WriteLine($"Order tapılmadı. OrderId: {orderId}");
                    return;
                }

                if (order.OrderStatus == OrderStatus.Paid)
                {
                    Console.WriteLine($"Order artıq Paid-dir. OrderId: {orderId}");
                    return;
                }

                var payment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == orderId);

                if (payment == null)
                {
                    Console.WriteLine($"Payment tapılmadı. OrderId: {orderId}");
                    return;
                }

                if (payment.Status == PaymentStatus.Success)
                {
                    Console.WriteLine($"Payment artıq success-dir. OrderId: {orderId}");
                    return;
                }

                order.OrderStatus = OrderStatus.Paid;
                payment.Status = PaymentStatus.Success;
                payment.PaidAt = DateTime.UtcNow;
                payment.ProviderReference = session.PaymentIntentId ?? session.Id;
                payment.FailureReason = null;

                var affected = await _unitOfWork.SaveChangesAsync();

                Console.WriteLine($"Webhook save nəticəsi: {affected}");
                Console.WriteLine("✅ Payment və Order uğurla update olundu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HandleCheckoutSessionCompletedAsync xətası: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task<IDataResult<PaymentGetDto>> MarkFailedAsync(string userId, PaymentMarkFailedDto dto)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == dto.OrderId && o.AppUserId == userId,
                "Payment",
                "OrderItems"
            );

            if (order == null)
                return new ErrorDataResult<PaymentGetDto>(null, "Order tapılmadı.");

            if (order.Payment == null)
                return new ErrorDataResult<PaymentGetDto>(null, "Bu order üçün payment tapılmadı.");

            if (order.Payment.Status == PaymentStatus.Success)
                return new ErrorDataResult<PaymentGetDto>(null, "Uğurlu payment-i failed etmək olmaz.");

            var productIds = order.OrderItems.Select(x => x.ProductId).Distinct().ToList();
            var products = await _unitOfWork.ProductRepository.GetAllAsync(p => productIds.Contains(p.Id));

            foreach (var oi in order.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == oi.ProductId);
                if (product != null)
                {
                    product.StockQuantity += oi.Quantity;
                }
            }

            order.Payment.Status = PaymentStatus.Failed;
            order.Payment.FailureReason = dto.FailureReason;

            var affected = await _unitOfWork.SaveChangesAsync();

            if (affected <= 0)
                return new ErrorDataResult<PaymentGetDto>(null, "Payment update alınmadı.");

            var paymentDto = _mapper.Map<PaymentGetDto>(order.Payment);
            return new SuccessDataResult<PaymentGetDto>(paymentDto, "Payment failed edildi.");
        }

        public async Task<IDataResult<PaymentGetDto>> MarkSuccessAsync(string userId, PaymentMarkSuccessDto dto)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == dto.OrderId && o.AppUserId == userId,
                "Payment"
            );

            if (order == null)
                return new ErrorDataResult<PaymentGetDto>(null, "Order tapılmadı.");

            if (order.Payment == null)
                return new ErrorDataResult<PaymentGetDto>(null, "Bu order üçün payment tapılmadı.");

            if (order.Payment.Status == PaymentStatus.Success)
                return new ErrorDataResult<PaymentGetDto>(null, "Payment artıq uğurludur.");

            order.Payment.Status = PaymentStatus.Success;
            order.Payment.PaidAt = DateTime.UtcNow;
            order.Payment.ProviderReference = dto.ProviderReference;
            order.Payment.FailureReason = null;

            order.OrderStatus = OrderStatus.Paid;

            var affected = await _unitOfWork.SaveChangesAsync();

            if (affected <= 0)
                return new ErrorDataResult<PaymentGetDto>(null, "Payment update alınmadı.");

            var paymentDto = _mapper.Map<PaymentGetDto>(order.Payment);
            return new SuccessDataResult<PaymentGetDto>(paymentDto, "Payment uğurlu edildi.");
        }
        public async Task HandlePaymentIntentSucceededAsync(Stripe.PaymentIntent intent)
        {
            try
            {
                if (intent == null) return;

                // PaymentIntent-in Metadata-sından orderId-ni oxuyuruq
                if (intent.Metadata == null || !intent.Metadata.ContainsKey("orderId"))
                {
                    Console.WriteLine("PaymentIntent gəldi, amma Metadata-da orderId yoxdur.");
                    return;
                }

                if (!int.TryParse(intent.Metadata["orderId"], out int orderId)) return;

                // Mövcud məntiqimizi işə salırıq (təkrar kod yazmamaq üçün)
                // Bunun üçün sessiya obyekti simulyasiya edirik və ya birbaşa bazanı yeniləyirik
                var order = await _unitOfWork.OrderRepository.GetAsync(x => x.Id == orderId);
                var payment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == orderId);

                if (order != null && payment != null && order.OrderStatus != OrderStatus.Paid)
                {
                    order.OrderStatus = OrderStatus.Paid;
                    payment.Status = PaymentStatus.Success;
                    payment.PaidAt = DateTime.UtcNow;
                    payment.ProviderReference = intent.Id;

                    await _unitOfWork.SaveChangesAsync();
                    Console.WriteLine($"✅ PaymentIntent vasitəsilə Order {orderId} Paid edildi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HandlePaymentIntentSucceededAsync xətası: {ex.Message}");
            }
        }
    }
}