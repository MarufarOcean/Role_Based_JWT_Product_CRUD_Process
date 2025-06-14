﻿using CRUD_Process.DTOs;
using CRUD_Process.Models;
using CRUD_Process.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Process.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        public OrderController(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderRepo.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                var newOrder = new Order
                {
                    ProductId = orderDto.ProductId,
                    Quantity = orderDto.Quantity,
                };

                var result = await _orderRepo.CreateOrderAsync(newOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderCreateDto orderDto)
        {
            try
            {
                var updatedOrder = new Order
                {
                    Id = id,
                    ProductId = orderDto.ProductId,
                    Quantity = orderDto.Quantity,
                    OrderDate = DateTime.Now // Or keep the original date if needed
                };

                var result = await _orderRepo.UpdateOrderAsync(updatedOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderRepo.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}   
