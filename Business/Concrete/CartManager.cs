using AutoMapper;
using Business.Abstract;
using Business.Dtos.Cart;
using Business.Dtos.CartItem;
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using DAL.UnitOfWork.Concrete;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class CartManager : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartManager(IUnitOfWork unitOfWork1, IMapper mapper)
        {
            _unitOfWork = unitOfWork1;
            _mapper = mapper;
        }

        public async Task<IResult> AddToCartAsync(string userId, CartAddDto dto)
        {
            var product = await _unitOfWork.ProductRepository
           .GetAsync(p => p.Id == dto.ProductId);
            if (product == null)
                return new ErrorResult("Product tapılmadı");
            if (product.StockQuantity < dto.Quantity)
                return new ErrorResult("Stock kifayət deyil");
            var cart = await _unitOfWork.CartRepository.GetAsync(
            c => c.AppUserId == userId,
            "CartItems"
        );

            if (cart == null)
            {
                cart = new Cart
                {
                    AppUserId = userId,
                    CartItems = new List<CartItem>()
                };
                await _unitOfWork.CartRepository.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }
            var existingItem = cart.CartItems
           .FirstOrDefault(x => x.ProductId == dto.ProductId);

            if (existingItem == null)
            {
                await _unitOfWork.CartitemRepository.AddAsync(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }
            else
            {
                var newQuantity = existingItem.Quantity + dto.Quantity;

                if (product.StockQuantity < newQuantity)
                    return new ErrorResult("Stock kifayət deyil (toplam quantity)");

                existingItem.Quantity = newQuantity;
                existingItem.UnitPrice = product.Price;

                _unitOfWork.CartitemRepository.Update(existingItem);
            }

            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Səbət yeniləndi");
        }
        public async Task<IResult> ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.CartRepository.GetAsync(
            c => c.AppUserId == userId,
            "CartItems"
        );

            if (cart == null)
                return new SuccessResult("Cart boşdur");

            foreach (var item in cart.CartItems)
                _unitOfWork.CartitemRepository.Remove(item);

            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Cart təmizləndi");
        }

        public async Task<IDataResult<CartGetDto>> GetMyCartAsync(string userId)
        {
            var cart = await _unitOfWork.CartRepository.
                GetAsync(c=>c.AppUserId==userId,"CartItems","CartItems.Product",
                "CartItems.Product.ProductImages");

            if (cart == null)
                return new SuccessDataResult<CartGetDto>(new CartGetDto
                {
                    CartId = 0,
                    Items = new List<CartItemGetDto>(),
                    TotalPrice = 0
                });

            var dto = _mapper.Map<CartGetDto>(cart);

            return new SuccessDataResult<CartGetDto>(dto);

        }

        public async Task<IResult> RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await _unitOfWork.CartRepository.GetAsync(p => p.AppUserId == userId, "CartItems");
            if (cart == null)
                return new ErrorResult("Cart Not Found");
            var item = cart.CartItems.FirstOrDefault(p => p.ProductId == productId);
            if (item == null)
                return new ErrorResult("CartItem Not Found");
            _unitOfWork.CartitemRepository.Remove(item);
            await _unitOfWork.SaveChangesAsync();
            return new SuccessResult("Mehsul sebetden silindi");

        }

        public async Task<IResult> UpdateCartAsync(string userId, CartUpdateDto dto)
        {

            var cart = await _unitOfWork.CartRepository.GetAsync(c => c.AppUserId == userId, "CartItems");
            if (cart == null) return new ErrorResult("Səbət tapılmadı.");

            var item = cart.CartItems.FirstOrDefault(x => x.ProductId == dto.ProductId);
            if (item == null) return new ErrorResult("Məhsul səbətdə yoxdur.");
            if (dto.Quantity <= 0)
            {
                _unitOfWork.CartitemRepository.Remove(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
                _unitOfWork.CartitemRepository.Update(item);
            }

            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0) return new ErrorResult("Update failed");

            return new SuccessResult("Cart updated");
        }
    }
}
