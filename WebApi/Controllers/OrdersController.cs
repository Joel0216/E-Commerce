using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Enums;
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
            await _service.AddOrderAsync(dto);
            return Ok();
        }

        [HttpPost("{orderId}/items")]
        public async Task<IActionResult> AddItem(int orderId, [FromBody] OrderItemDto dto)
        {
            await _service.AddItemAsync(orderId, dto);
            return Ok();
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] OrderStatus status)
        {
            await _service.UpdateStatusAsync(orderId, status);
            return Ok();
        }
    }
}
