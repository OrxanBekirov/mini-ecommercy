using Business.Abstract;
using Business.Dtos.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var result = await _cartService.GetMyCartAsync(UserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartAddDto dto)
        {
            var result = await _cartService.AddToCartAsync(UserId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var result = await _cartService.RemoveFromCartAsync(UserId, productId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            var result = await _cartService.ClearCartAsync(UserId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart([FromBody] CartUpdateDto dto)
        {
            
            var result = await _cartService.UpdateCartAsync(UserId, dto);

            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}
