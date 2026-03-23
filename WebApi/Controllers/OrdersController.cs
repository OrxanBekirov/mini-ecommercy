using Business.Abstract;
using Business.Dtos.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User id tapılmadı.");

            var result = await _orderService.CreateAsync(userId, dto);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User id tapılmadı.");

            var result = await _orderService.GetByIdAsync(id, userId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        // GET: api/orders/my
        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User id tapılmadı.");

            var result = await _orderService.GetMyOrdersAsync(userId);

            return Ok(result);
        }
        [HttpPost("from-cart")]
        public async Task<IActionResult> CreateFromCart([FromBody] OrderFromCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User id tapılmadı.");

            var result = await _orderService.CreateFromCartAsync(userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] OrderCancelDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User id tapılmadı.");

            var result = await _orderService.CancelAsync(id, userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StatusUpdate(int id, [FromBody] OrderStatusUpdateDto dto)
        {

            var result = await _orderService.UpdateStatusAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
