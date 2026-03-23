using Business.Abstract;
using Business.Dtos.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }
        // Admin panel üçün (məs: product şəkli, brand logo)
        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] MediaCreateDto dto)
        {
            var result = await _mediaService.UploadAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // admin silmə (istəsən user üçün ayrıca endpoint edərik)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediaService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("GetByOwner")]
        public async Task<IActionResult> GetByOwner([FromQuery] Entities.Enum.MediaOwnerType ownerType, [FromQuery] int? ownerId, [FromQuery] string? ownerKey)
        {
            var result = await _mediaService.GetByOwnerAsync(ownerType, ownerId, ownerKey);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

}
