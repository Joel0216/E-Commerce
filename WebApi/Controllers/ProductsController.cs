using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        // ✔ Acceso libre
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

        // ✔ Acceso libre
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        // 🔐 Requiere token
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        // 🔐 Requiere token
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok();
        }

        // 🔐 Requiere token
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}
