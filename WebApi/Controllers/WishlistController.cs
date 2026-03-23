using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var result = await _wishlistService.GetMyWishlistAsync();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var result = await _wishlistService.AddToWishlistAsync(productId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var result = await _wishlistService.RemoveFromWishlistAsync(productId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
