using System;
using System.Collections.Generic;
using System.Text;
using Core.Security;
using Entities.Concrete.Auth;
namespace Business.Abstract
{
    public interface ITokenService
    {
        Task<AccesToken> CreateAccessTokenAsync(AppUser user);
    }
}
