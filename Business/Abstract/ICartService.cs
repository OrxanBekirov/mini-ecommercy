using Business.Dtos.Cart;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface ICartService
    {
        Task<IDataResult<CartGetDto>> GetMyCartAsync(string userId);
        Task<IResult> AddToCartAsync(string userId, CartAddDto dto);
        Task<IResult> RemoveFromCartAsync(string userId, int productId);
        Task<IResult> ClearCartAsync(string userId);
        Task<IResult> UpdateCartAsync(string userId, CartUpdateDto dto);
    }
}
