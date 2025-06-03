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
        public async Task<Order> UpdateOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == order.Id);
            if (existingOrder == null)
                throw new Exception("Order not found.");

            var product = await _context.Products.FindAsync(order.ProductId);
            if (product == null)
                throw new Exception("Invalid Product ID.");

            // Restore stock from previous order
            var oldProduct = await _context.Products.FindAsync(existingOrder.ProductId);
            if (oldProduct != null)
            {
                oldProduct.stock = (Convert.ToInt32(oldProduct.stock) + existingOrder.Quantity).ToString();
                _context.Products.Update(oldProduct);
            }

            // Check new stock availability
            var newStock = Convert.ToInt32(product.stock);
            if (newStock < order.Quantity)
                throw new Exception("Not enough stock available for the selected product.");

            // Deduct new quantity
            product.stock = (newStock - order.Quantity).ToString();
            _context.Products.Update(product);

            // Update order fields
            existingOrder.ProductId = order.ProductId;
            existingOrder.Quantity = order.Quantity;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.Product = product;

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();

            return existingOrder;
        }
        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.Product).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
                throw new Exception("Order not found.");

            // Restore stock
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product != null)
            {
                product.stock = (Convert.ToInt32(product.stock) + order.Quantity).ToString();
                _context.Products.Update(product);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
