using CRUD_Process.DBContext;
using CRUD_Process.DTOs;
using CRUD_Process.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Process.Repository
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                .Include(s => s.SupplierProducts)
                .ThenInclude(sp => sp.Product)
                .ToListAsync();
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.SupplierProducts)
                .ThenInclude(sp => sp.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier> AddAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier> UpdateAsync(Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers.FindAsync(supplier.Id);
            if (existingSupplier == null)
                throw new Exception("Supplier not found");

            existingSupplier.Name = supplier.Name;
            existingSupplier.ContactInfo = supplier.ContactInfo;

            _context.Suppliers.Update(existingSupplier);
            await _context.SaveChangesAsync();

            return existingSupplier;
        }

        public async Task<IEnumerable<SupplierProductDto>> GetAllSuppliesWithDetailsAsync()
        {
            var supplies = await _context.SupplierProducts
                .Include(sp => sp.Supplier)
                .Include(sp => sp.Product)
                .ToListAsync();

            return supplies.Select(sp => new SupplierProductDto
            {
                Id = sp.Id,
                SupplierName = sp.Supplier.Name,
                ProductName = sp.Product.Name,
                Quantity = sp.Quantity
            }).ToList();
        }

        public async Task<SupplierProduct> AddSuppliedProductAsync(SupplierProduct suppliedProduct)
        {
            var product = await _context.Products.FindAsync(suppliedProduct.ProductId);
            var stock = Convert.ToInt32(product.stock);
            if (product == null) throw new Exception("Invalid Product");

            // স্টকে যোগ করো
            stock += suppliedProduct.Quantity;
            product.stock = stock.ToString();
            // Product entity 'modified'
            _context.Products.Update(product);

            _context.SupplierProducts.Add(suppliedProduct);
            await _context.SaveChangesAsync();
            return suppliedProduct;
        }

        public async Task<SupplierProduct> UpdateSuppliedProductAsync(SupplierProduct suppliedProduct)
        {
            var existing = await _context.SupplierProducts.FindAsync(suppliedProduct.Id);
            if (existing == null)
                return null;

            // Quantity difference calculation
            var quantityDiff = suppliedProduct.Quantity - existing.Quantity;

            existing.ProductId = suppliedProduct.ProductId;
            existing.Quantity = suppliedProduct.Quantity;
            existing.SuppliedDate = DateTime.Now;

            // Update stock
            var product = await _context.Products.FindAsync(existing.ProductId);
            var stock = Convert.ToInt32(product.stock);
            stock += quantityDiff;
            product.stock = stock.ToString();
            _context.Products.Update(product);

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
