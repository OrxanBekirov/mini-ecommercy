using Business.Abstract;
using Business.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] UserProfilImageUploadDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = userIdClaim;

            var result = await _userService.UploadFrofilImageAsync(userId, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
             [Authorize]
        [HttpGet("my-profile-image")]
        public async Task<IActionResult> GetMyProfileImage()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = userIdClaim;

            var result = await _userService.GetFrofilImage(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }

    }
