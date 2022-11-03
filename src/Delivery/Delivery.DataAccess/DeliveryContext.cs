using Delivery.DataAccess.Dto;
using Microsoft.EntityFrameworkCore;

namespace Delivery.DataAccess
{
    public class DeliveryContext : DbContext
    {
        public DeliveryContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<DeliveryItem> DeliveryItems { get; set; }
    }
}
