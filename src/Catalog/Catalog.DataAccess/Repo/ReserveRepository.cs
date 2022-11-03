using Catalog.DataAccess.Abstraction;
using Microsoft.EntityFrameworkCore;
using Models;


namespace Catalog.DataAccess.Repo
{
    public class ReserveRepository : IReserveRepository
    {
        private readonly CatalogContext dbContext;

        public ReserveRepository(CatalogContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<OrderItem>> GetAll()
            => await dbContext.ReserveProducts.ToListAsync();

        public async Task<IEnumerable<OrderItem>> GetReserveProducts(Guid orderId)
            => await dbContext.ReserveProducts
                        .AsNoTracking()
                        .Where(x => x.OrderId == orderId)
                        .ToListAsync();

        public async Task MakeReservation(IEnumerable<OrderItem> orderItems)
        {
            if (orderItems is null || !orderItems.Any())
                return;

            await dbContext.AddRangeAsync(orderItems);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveItems(IEnumerable<OrderItem> orderItems)
        {
            if (orderItems is null || !orderItems.Any())
                return;

            dbContext.ReserveProducts.RemoveRange(orderItems);
            await dbContext.SaveChangesAsync();
        }
    }
}
