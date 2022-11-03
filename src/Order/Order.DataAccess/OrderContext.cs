using Microsoft.EntityFrameworkCore;
using Models;

namespace Order.DataAccess
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
