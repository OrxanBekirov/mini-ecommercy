
using Business.Abstract;
using Core.Security;
using Entities.Concrete.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Concrete
{
    public class TokenManager : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenOption _tokenOption;
       

        public TokenManager(UserManager<AppUser> userManager, IOptions<TokenOption> tokenOption)
        {
            _userManager = userManager;
            _tokenOption = tokenOption.Value;
        }

        public async Task<AccesToken> CreateAccessTokenAsync(AppUser user)
        {
            // 1) key + creds + header
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOption.SecurityKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);


            var header = new JwtHeader(signingCredentials);//2

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.GivenName, user.FullName ?? "") // FullName
        };

            foreach (var item in await _userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            JwtPayload payload = new JwtPayload
                (issuer: _tokenOption.Issuer,
                audience: _tokenOption.Audience, claims: claims, notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration));

            var securityToken = new JwtSecurityToken(header, payload);//1

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string jwt = tokenHandler.WriteToken(securityToken);

            return new AccesToken
            {
                Token= jwt,
                  Expiration = DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration)

        };

        }
    }
}
