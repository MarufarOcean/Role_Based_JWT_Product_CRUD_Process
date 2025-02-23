using CRUD_Process.Models;
using Microsoft.EntityFrameworkCore;
namespace CRUD_Process.DBContext
{
    
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
