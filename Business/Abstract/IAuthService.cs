using Business.Dtos.Auth;
using Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<IDataResult<AuthResponseDto>> RegisterAsync(RegisterDto dto);
        Task<IDataResult<AuthResponseDto>> LoginAsync(LoginDto dto);
    }
}
