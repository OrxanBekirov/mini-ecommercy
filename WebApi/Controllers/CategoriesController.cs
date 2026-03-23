using Busines.Utilities.Contants;
using Business.Abstract;
using Business.Dtos.Brand;
using Business.Dtos.Category;
using Core.Result.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CategoryCreateDto dto)
        {
           
            var result = await _categoryService.AddAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        [HttpGet("pagination")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _categoryService.GetAllPaginationAsync(pageNumber, pageSize);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
