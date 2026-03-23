using Business.Abstract;
using Business.Dtos.Brand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _brandService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _brandService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] BrandCreateDto dto)
        {
            var result = await _brandService.AddAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandUpdateDto dto)
        {
            var result = await _brandService.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}