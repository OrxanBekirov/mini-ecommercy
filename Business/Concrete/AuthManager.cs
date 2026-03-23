using Busines.Utilities.Contants;
using Business.Abstract;
using Business.Dtos.Auth;
using Core.Result.Abstract;
using Core.Result.Concrete;
using Entities.Concrete.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthManager(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }
        public async Task<IDataResult<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return new ErrorDataResult<AuthResponseDto>(null, ExceptionsMessage.AlreadyRegistred);
            var user = new AppUser
            {
                Email = dto.Email,
                UserName = dto.UserName,
                FullName = dto.FullName
            };

            var addedUser = await _userManager.CreateAsync(user, dto.Password);
            if (!addedUser.Succeeded)
            {
                // Identity-dən gələn bütün xətaların təsvirini bir listə yığırıq
                var errorDescriptions = addedUser.Errors.Select(x => x.Description).ToList();

                // "xetalidir" əvəzinə həmin listi göndəririk
                return new ErrorDataResult<AuthResponseDto>(null, string.Join(", ", errorDescriptions));
            }
            //role
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");
            // token
            var accessToken = await _tokenService.CreateAccessTokenAsync(user);

            return new SuccessDataResult<AuthResponseDto>(new AuthResponseDto
            {
                Token = accessToken.Token,
                Expires = accessToken.Expiration
            }, "Register uğurludur");
        }
        public async Task<IDataResult<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new ErrorDataResult<AuthResponseDto>(null,"Email və ya şifrə yanlışdır");

            var ok = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!ok)
                return new ErrorDataResult<AuthResponseDto>(null,"Email və ya şifrə yanlışdır");

            var accessToken = await _tokenService.CreateAccessTokenAsync(user);

            return new SuccessDataResult<AuthResponseDto>(new AuthResponseDto
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                ProfileImageUrl = user.ProfileImageUrl,
                Token = accessToken.Token,
                Expires = accessToken.Expiration
            }, "Login uğurludur");
        }

        
    }
}
