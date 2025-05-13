using CRUD_Process.DTOs;
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
            if (product == null) return NotFound();

            // Read the photo file and convert it to a Base64 string
            string base64Photo = null;
            if (!string.IsNullOrEmpty(product.PhotoUrl))
            {
                var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.PhotoUrl.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                {
                    var photoBytes = await System.IO.File.ReadAllBytesAsync(photoPath);
                    base64Photo = Convert.ToBase64String(photoBytes);
                    product.PhotoUrl = base64Photo;
                }
            }

            return Ok(product);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] ProductInputDto productInput)
        {
            var product = new Product
            {
                Name = productInput.Name,
                Description = productInput.Description,
                Details = productInput.Details,
                Price = productInput.Price,
                stock = productInput.stock
            };

            if (productInput.Photo != null)
            {
                var photoUrl = await SavePhotoAsync(productInput.Photo);
                product.PhotoUrl = photoUrl;
            }

            await _productRepository.Add(product);
            return Ok(product);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductInputDto productInput)
        {
            var existingProduct = await _productRepository.GetById(id);
            if (existingProduct == null) return NotFound();

            existingProduct.Name = productInput.Name;
            existingProduct.Description = productInput.Description;
            existingProduct.Price = productInput.Price;
            existingProduct.stock = productInput.stock;
            existingProduct.Details = productInput.Details;

            if (productInput.Photo != null)
            {
                var photoUrl = await SavePhotoAsync(productInput.Photo);
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
