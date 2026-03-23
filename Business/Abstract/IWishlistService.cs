using Business.Dtos.Wishlist;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IWishlistService
    {
        Task<IDataResult<List<WishlistGetDto>>> GetMyWishlistAsync();

        Task<IResult> AddToWishlistAsync(int productId);

        Task<IResult> RemoveFromWishlistAsync(int productId);


    }
}
