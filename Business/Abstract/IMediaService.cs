using Business.Dtos.Media;
using Core.Result.Abstract;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IMediaService
    {
        Task<IDataResult<MediaUploadResultDto>> UploadAsync(MediaCreateDto dto);
        Task<IResult> DeleteAsync(int mediaId);
        Task<IDataResult<List<MediaUploadResultDto>>> 
            GetByOwnerAsync(MediaOwnerType ownerType, int? ownerId, string ownerKey);
    }
}
