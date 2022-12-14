using Catalog.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Models;


namespace Catalog.DataAccess
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ReservedItem> ReservedItems { get; set; }
    }
}
