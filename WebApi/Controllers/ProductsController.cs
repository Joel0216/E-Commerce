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
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;
            if (pageSize > 25) pageSize = 25; // Máximo 25 elementos
            
            var result = await _service.GetAllAsync(page, pageSize);
            if (result == null || result.Items == null)
                return NotFound(new { Message = "No se encontraron productos" });
            return Ok(result);
        }

        // ✔ Acceso libre
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { Message = "Producto no encontrado" });
            return Ok(product);
        }

        // 🔐 Requiere token
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    })
                    .ToList();

                return BadRequest(new
                {
                    Message = "Error de validación",
                    Errors = errors
                });
            }

            await _service.AddAsync(dto);
            return Ok();
        }

        // 🔐 Requiere token
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    })
                    .ToList();

                return BadRequest(new
                {
                    Message = "Error de validación",
                    Errors = errors
                });
            }

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
