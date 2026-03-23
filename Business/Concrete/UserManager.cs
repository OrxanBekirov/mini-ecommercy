using Business.Abstract;
using Business.Dtos.Auth;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Result.Abstract;
using Core.Result.Concrete;
using Entities.Concrete.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Cloudinary _cloudinary;

        public UserManager(UserManager<AppUser> userManager, Cloudinary cloudinary)
        {
            _userManager = userManager;
            _cloudinary = cloudinary;
        }

        public async Task<IDataResult<string>> GetFrofilImage(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new ErrorDataResult<string>("İstifadəçi tapılmadı");

            return new SuccessDataResult<string>(user.ProfileImageUrl);
        }

        public async Task<IResult> UploadFrofilImageAsync(string userId, UserProfilImageUploadDto dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return new ErrorResult("İstifadəçi tapılmadı");

            if (dto.File == null || dto.File.Length == 0)
                return new ErrorResult("Şəkil seçilməyib");

            ImageUploadResult uploadResult;

            await using (var stream = dto.File.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(dto.File.FileName, stream),
                    Folder = "users/profile-images",
                    PublicId = $"user_{user.Id}_{Guid.NewGuid()}",
                    Overwrite = true
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK &&
                uploadResult.StatusCode != System.Net.HttpStatusCode.Created)
            {
                return new ErrorResult("Şəkil yüklənə bilmədi");
            }

            user.ProfileImageUrl = uploadResult.SecureUrl?.ToString();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ErrorResult("İstifadəçi yenilənə bilmədi");

            return new SuccessResult("Profil şəkli uğurla yeniləndi");
        }
    }
}
