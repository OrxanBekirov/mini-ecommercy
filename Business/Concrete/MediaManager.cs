using AutoMapper;
using Business.Abstract;
using Business.Dtos.Media;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Result.Abstract;
using Core.Result.Concrete;
using DAL.UnitOfWork.Abstract;
using Entities.Concrete;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Business.Concrete
{
    public class MediaManager : IMediaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public MediaManager(IUnitOfWork unitOfWork, Cloudinary cloudinary, IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
            _mapper = mapper;
        }

        public async Task<IResult> DeleteAsync(int mediaId)
        {

            var media = await _unitOfWork.MediaRepository.GetAsync(m => m.Id == mediaId);
            if (media == null)
                return new ErrorResult("Media tapılmadı.");

            // Cloudinary delete
            var delParams = new DeletionParams(media.PublicId)
            {
                ResourceType = ResourceType.Image
            };

            var delResult = await _cloudinary.DestroyAsync(delParams);

            // Cloudinary bəzən "not found" qaytara bilər. Biz yenə DB-dən silmək istəyirik.
            // delResult.Result "ok" olmasa da, DB silməyə icazə verə bilərik.
            _unitOfWork.MediaRepository.Remove(media); // səndə Delete/Remove necədirsə ona uyğun

            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0)
                return new ErrorResult("DB-dən silinmədi.");

            return new SuccessResult("Şəkil silindi.");
        }

        public async Task<IDataResult<MediaUploadResultDto>> UploadAsync(MediaCreateDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return new ErrorDataResult<MediaUploadResultDto>(null, "Fayl boşdur.");

            // Owner validation
            if (dto.OwnerType == MediaOwnerType.User)
            {
                if (string.IsNullOrWhiteSpace(dto.OwnerKey))
                    return new ErrorDataResult<MediaUploadResultDto>(null, "User üçün OwnerKey tələb olunur.");
                dto.OwnerId = null; // user üçün ownerId lazım deyil
            }
            else
            {
                if (dto.OwnerId == null || dto.OwnerId <= 0)
                    return new ErrorDataResult<MediaUploadResultDto>(null, "Bu owner üçün OwnerId tələb olunur.");
                dto.OwnerKey = null; // product/brand/category/banner üçün ownerKey lazım deyil
            }

            // IsMain true gəlibsə əvvəlki main-ləri söndür
            if (dto.IsMain)
            {
                var existing = await _unitOfWork.MediaRepository.GetAllAsync(
                    BuildOwnerFilter(dto.OwnerType, dto.OwnerId, dto.OwnerKey)
                );

                foreach (var m in existing)
                    m.IsMain = false;
            }

            // Cloudinary upload
            ImageUploadResult uploadResult;
            using (var stream = dto.File.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(dto.File.FileName, stream),
                    Folder = GetFolder(dto.OwnerType),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
                return new ErrorDataResult<MediaUploadResultDto>(null, uploadResult.Error.Message);

            var media = new Media
            {
                Url = uploadResult.SecureUrl?.ToString(),
                PublicId = uploadResult.PublicId,
                ResourceType = "image",
                IsMain = dto.IsMain,
                OwnerType = dto.OwnerType,
                OwnerId = dto.OwnerId,
                OwnerKey = dto.OwnerKey
            };

            await _unitOfWork.MediaRepository.AddAsync(media);

            var affected = await _unitOfWork.SaveChangesAsync();
            if (affected <= 0)
                return new ErrorDataResult<MediaUploadResultDto>(null, "DB-yə yazılmadı.");

            var resultDto = new MediaUploadResultDto
            {
                Id = media.Id,
                Url = media.Url,
                PublicId = media.PublicId,
                OwnerType = media.OwnerType,
                OwnerId = media.OwnerId,
                OwnerKey = media.OwnerKey,
                IsMain = media.IsMain
            };

            return new SuccessDataResult<MediaUploadResultDto>(resultDto, "Şəkil yükləndi.");
        }
        private static string GetFolder(MediaOwnerType ownerType)
        {
            return ownerType switch
            {
                MediaOwnerType.Product => "products",
                MediaOwnerType.User => "users",
                MediaOwnerType.Category => "categories",
                MediaOwnerType.Brand => "brands",
                MediaOwnerType.Banner => "banners",
                _ => "uploads"
            };
        }

        public async Task<IDataResult<List<MediaUploadResultDto>>> GetByOwnerAsync(MediaOwnerType ownerType, int? ownerId, string ownerKey)
        {

            var list = await _unitOfWork.MediaRepository.GetAllAsync(m =>
                m.OwnerType == ownerType &&
                m.OwnerId == ownerId &&
                m.OwnerKey == ownerKey
            );

            var dtos = _mapper.Map<List<MediaUploadResultDto>>(list);
            return new SuccessDataResult<List<MediaUploadResultDto>>(dtos);

        }
        private static Expression<Func<Media, bool>> BuildOwnerFilter(MediaOwnerType ownerType, int? ownerId, string ownerKey)
        {
            if (ownerType == MediaOwnerType.User)
                return m => m.OwnerType == ownerType && m.OwnerKey == ownerKey;

            return m => m.OwnerType == ownerType && m.OwnerId == ownerId;
        }
    }
}
