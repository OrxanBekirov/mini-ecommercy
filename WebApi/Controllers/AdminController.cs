using Business.Abstract;
using Business.Dtos.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateDto dto)
        {
            var result = await _orderService.UpdateStatusAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
