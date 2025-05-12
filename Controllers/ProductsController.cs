using CRUD_Process.Models;
using CRUD_Process.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Process.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;

        public ProductsController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetAll();
            return Ok(products);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductsById(int id)
        {
            var product = await _productRepository.GetById(id);
            return Ok(product);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] Product product, [FromForm] IFormFile photo)
        {
            if (photo != null)
            {
                // Save the photo and set the PhotoUrl property
                var photoUrl = await SavePhotoAsync(photo);
                product.PhotoUrl = photoUrl;
            }

            await _productRepository.Add(product);
            return Ok(product);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product, [FromForm] IFormFile photo)
        {
            var existingProduct = await _productRepository.GetById(id);
            if (existingProduct == null) return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.stock = product.stock;
            existingProduct.Details = product.Details;

            if (photo != null)
            {
                // Save the new photo and update the PhotoUrl property
                var photoUrl = await SavePhotoAsync(photo);
                existingProduct.PhotoUrl = photoUrl;
            }

            await _productRepository.Update(existingProduct);
            return Ok(existingProduct);
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productRepository.Delete(id);
            return Ok();
        }

        //save photo to the server
        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return $"/photos/{fileName}"; // Return the relative URL
        }
    }
}
