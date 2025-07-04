using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
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

            await _service.AddOrderAsync(dto);
            return Ok(new { Message = "Orden creada exitosamente" });
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> AddItem(int orderId, [FromBody] OrderItemDto dto)
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

            await _service.AddItemAsync(orderId, dto);
            return Ok(new { Message = "Item agregado exitosamente" });
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
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

            if (!Enum.TryParse<Domain.Enums.OrderStatus>(dto.Status, out var newStatus))
            {
                return BadRequest(new { Message = "Status inválido. Usa: Pending, Paid o Shipped" });
            }

            try
            {
                await _service.UpdateStatusAsync(orderId, newStatus);
                return Ok(new { Message = "Status actualizado correctamente" });
            }
            catch (Application.Exceptions.BusinessException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // ✅ Nuevo método GET /api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            if (orders == null || !orders.Any())
                return NotFound(new { Message = "No se encontraron órdenes" });
            return Ok(orders);
        }
    }
}
