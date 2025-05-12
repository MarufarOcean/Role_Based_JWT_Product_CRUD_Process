namespace CRUD_Process.DTOs
{
    public class ProductInputDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public string stock { get; set; }
        public IFormFile Photo { get; set; } // Only include the photo input
    }
}
