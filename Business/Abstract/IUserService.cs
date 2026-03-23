using Business.Dtos.Auth;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IUserService
    {
        Task<IResult>UploadFrofilImageAsync(string userId,UserProfilImageUploadDto dto);
        Task<IDataResult<string>> GetFrofilImage(string userId);
    }
}
