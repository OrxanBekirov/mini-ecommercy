using AutoMapper;
using Business.Abstract;
using Business.Dtos.Wishlist;
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using Entities.Concrete;
using Entities.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using IResult = Core.Result.Abstract.IResult;

namespace Business.Concrete
{
    public class WishlistManager : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WishlistManager(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        private string GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return "";

            return userIdClaim;
        }
        public async Task<IResult> AddToWishlistAsync(int productId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new ErrorResult("İstifadəçi tapılmadı");

            var product = await _unitOfWork.ProductRepository.GetAsync(x => x.Id == productId && !x.IsDeleted);
            if (product == null)
                return new ErrorResult("Məhsul tapılmadı");
            var isExist = await _unitOfWork.WishlistRepository.IsExistAsync(w => w.AppUserId == userId && w.ProductId == productId);

            if (isExist)
            {
                return new ErrorResult("Bu məhsul artıq favoritlərinizdə var.");
            }

            var wishlist = new Wishlist
            {
                AppUserId = userId,
                ProductId = productId
            };

            await _unitOfWork.WishlistRepository.AddAsync(wishlist);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Məhsul wishlist-ə əlavə olundu");
        }

        public async Task<IDataResult<List<WishlistGetDto>>> GetMyWishlistAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new ErrorDataResult<List<WishlistGetDto>>(null, "İstifadəçi tapılmadı");

            // 1. Wishlist-i və əlaqəli Product məlumatlarını çəkirik
            var wishlistItems = await _unitOfWork.WishlistRepository.GetAllAsync(
                x => x.AppUserId == userId,
                "Product.Brand", "Product.Category"
            );

            if (wishlistItems == null || !wishlistItems.Any())
                return new SuccessDataResult<List<WishlistGetDto>>(new List<WishlistGetDto>(), "Wishlist boşdur");

            // 2. Wishlist-dəki bütün Product ID-lərini götürürük
            var productIds = wishlistItems.Select(x => x.ProductId).ToList();

            // 3. MediaRepository-dən həmin məhsullara aid şəkilləri çəkirik
            var medias = await _unitOfWork.MediaRepository.GetAllAsync(
                m => m.OwnerType == MediaOwnerType.Product // Sənin Enum-un
                     && m.OwnerId.HasValue
                     && productIds.Contains(m.OwnerId.Value)
                     && !m.IsDeleted
            );

            // 4. Məlumatları DTO-ya çeviririk
            var result = wishlistItems.Select(x => new WishlistGetDto
            {
                WishlistId = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Description = x.Product.Description,
                Price = x.Product.Price,
                BrandId = x.Product.BrandId,
                BrandName = x.Product.Brand?.Name,
                CategoryId = x.Product.CategoryId,
                CategoryName = x.Product.Category?.Name,
                // Şəkilləri Media cədvəlindən gələn datadan filter edirik
                ImageUrls = medias
                    .Where(m => m.OwnerId == x.ProductId)
                    .OrderByDescending(m => m.IsMain)
                    .Select(m => m.Url)
                    .ToList()
            }).ToList();

            return new SuccessDataResult<List<WishlistGetDto>>(result, "Wishlist gətirildi");
        }
        public async Task<IResult> RemoveFromWishlistAsync(int productId)
        {
            // 1. İstifadəçi yoxlaması
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return new ErrorResult("İstifadəçi tapılmadı"); // DataResult yox, sadəcə ErrorResult

            // 2. Mütləq 'await' istifadə etməlisən
            var wishlistItem = await _unitOfWork.WishlistRepository.GetAsync(w => w.AppUserId == userId && w.ProductId == productId);

            if (wishlistItem == null)
                return new ErrorResult("Məhsul sizin wishlist-inizdə tapılmadı");

            // 3. Silmə və Yaddaşa vermə
            _unitOfWork.WishlistRepository.Remove(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResult("Məhsul wishlist-dən silindi");
        }
    }
}
