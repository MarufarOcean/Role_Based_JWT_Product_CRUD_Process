using CRUD_Process.DTOs;
using CRUD_Process.Models;
using CRUD_Process.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Process.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _repo;

        public SupplierController(ISupplierRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repo.GetAllAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierCreateDto Sdto)
        {
            var supplier = new Supplier
            {
                Name = Sdto.Name,
                ContactInfo = Sdto.ContactInfo
            };

            var result = await _repo.AddAsync(supplier);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierUpdateDto dto)
        {

            try
            {
                var supplier = new Supplier
                {
                    Id = id,
                    Name = dto.Name,
                    ContactInfo = dto.ContactInfo
                };

                var result = await _repo.UpdateAsync(supplier);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpGet("supply")]
        public async Task<IActionResult> GetAllSupplies()
        {
            var suppliesDto = await _repo.GetAllSuppliesWithDetailsAsync();
            return Ok(suppliesDto);
        }

        [HttpPost("supply")]
        public async Task<IActionResult> AddSupply([FromBody] SupplierSupplyCreateDto dto)
        {
            var supply = new SupplierProduct
            {
                SupplierId = dto.SupplierId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
            };

            var result= await _repo.AddSuppliedProductAsync(supply);
            return Ok(result);
        }

        [HttpPut("supply/{id}")]
        public async Task<IActionResult> UpdateSuppliedProductAsync(int id, [FromBody] SupplierSupplyUpdateDto dto)
        {
            try
            {
                var supplyProduct = new SupplierProduct
                {
                    Id = id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                var result = await _repo.UpdateSuppliedProductAsync(supplyProduct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }
    }
}
