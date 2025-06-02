namespace CRUD_Process.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public string stock { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<SupplierProduct> SupplierProducts { get; set; }
    }
}
