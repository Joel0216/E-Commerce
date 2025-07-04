using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(new { 
                Message = "API funcionando correctamente",
                Timestamp = DateTime.Now,
                Status = "OK"
            });
        }

        [HttpGet("db")]
        [AllowAnonymous]
        public async Task<IActionResult> TestDatabase([FromServices] Infrastructure.Data.AppDbContext context)
        {
            try
            {
                var productCount = await context.Products.CountAsync();
                return Ok(new { 
                    Message = "Conexión a base de datos exitosa",
                    ProductCount = productCount,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Message = "Error de base de datos",
                    Error = ex.Message,
                    Timestamp = DateTime.Now
                });
            }
        }
    }
} 