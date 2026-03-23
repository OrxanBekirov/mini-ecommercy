using Business.Abstract;
using Business.Dtos.Contact;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] ContactSendDto dto)
        {
            var result = await _emailService.SentContactEmailAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}