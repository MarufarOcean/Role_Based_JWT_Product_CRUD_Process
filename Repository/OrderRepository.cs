using CRUD_Process.DBContext;
using CRUD_Process.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Process.Repository
{
    public class OrderRepository: IOrderRepository 
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders.Include(o => o.Product).ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            var product = await _context.Products.FindAsync(order.ProductId);
            var stock = Convert.ToInt32(product.stock);

            if (product == null)
                throw new Exception("Invalid Product ID");

            if (stock < order.Quantity)
                throw new Exception("Not enough stock available for the selected product.");

            // stock updated

            stock -= order.Quantity;
            product.stock = stock.ToString();
            // Product entity 'modified'
            _context.Products.Update(product);
            
            order.Product = product;
            order.OrderDate = DateTime.Now;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
