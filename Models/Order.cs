using System.ComponentModel.DataAnnotations;

namespace CRUD_Process.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }

        // Navigation Property
        public Product? Product { get; set; }
    }
}
