namespace CRUD_Process.Models
{
    public class SupplierProduct
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public DateTime SuppliedDate { get; set; }
    }
}
