using Business.Abstract;
using Business.Dtos.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("pagination")]
        public async Task<IActionResult> GetPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllPaginatedAsync(pageNumber, pageSize);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        // GET: /api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // POST: /api/products
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductCreateDto dto)
        {
            var result = await _service.AddAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // PUT: /api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // DELETE: /api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
