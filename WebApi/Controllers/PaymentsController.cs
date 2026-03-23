using Business.Abstract;
using Business.Dtos.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("mark-success")]
        public async Task<IActionResult> MarkSuccess([FromBody] PaymentMarkSuccessDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User id tapılmadı.");

            var result = await _paymentService.MarkSuccessAsync(userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("mark-failed")]
        public async Task<IActionResult> MarkFailed([FromBody] PaymentMarkFailedDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User id tapılmadı.");

            var result = await _paymentService.MarkFailedAsync(userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionDto dto)
        {
            var result = await _paymentService.CreateCheckoutSessionAsync(dto);

            if (!result.Success)
                return BadRequest(result);
            return Ok(new { url = result.Data });
            return Ok(result);
        }
    }
}
