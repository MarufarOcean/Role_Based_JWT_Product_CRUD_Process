using CRUD_Process.DTOs;
using CRUD_Process.Models;

namespace CRUD_Process.Repository
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier> GetByIdAsync(int id);
        Task<Supplier> AddAsync(Supplier supplier);
        Task<Supplier> UpdateAsync(Supplier supplier);

        //For supplied products
        Task<IEnumerable<SupplierProductDto>> GetAllSuppliesWithDetailsAsync();
        Task<SupplierProduct> AddSuppliedProductAsync(SupplierProduct suppliedProduct);
        Task<SupplierProduct> UpdateSuppliedProductAsync(SupplierProduct suppliedProduct);
    }
}
