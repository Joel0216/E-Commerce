using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using WebApi.Seeders;

namespace WebApi.Controllers;

[ApiController]
[Route("api/dev")]
public class DevController : ControllerBase
{
    private readonly AppDbContext _context;

    public DevController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public IActionResult RunSeeder()
    {
        try
        {
            DbSeeder.SeedDatabase(_context);
            return Ok("Seeder ejecutado correctamente.");
        }
        catch (Exception ex)
        {
            return BadRequest("Error al ejecutar el Seeder: " + ex.Message);
        }
    }
}
