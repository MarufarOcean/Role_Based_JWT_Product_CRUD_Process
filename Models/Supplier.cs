namespace CRUD_Process.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;

        public ICollection<SupplierProduct> SupplierProducts { get; set; }
    }
}
