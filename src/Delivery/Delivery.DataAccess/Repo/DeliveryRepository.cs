using Models;
using Microsoft.EntityFrameworkCore;
using Delivery.DataAccess.Abstraction;

namespace Delivery.DataAccess.Repo
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly DeliveryContext dbContext;

        public DeliveryRepository(DeliveryContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<DeliveryItem>> GetAllOrders()
            => await dbContext.DeliveryItems
                        .AsNoTracking()
                        .ToListAsync();

        public async Task<DeliveryItem?> GetOrder(Guid orderId)
            => await dbContext.DeliveryItems
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.OrderId == orderId);

        public async Task RecordOrder(DeliveryItem item)
        {
            await dbContext.DeliveryItems.AddAsync(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveOrder(Guid orderId)
        {
            var order = await dbContext.DeliveryItems.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order is null)
                return;

            dbContext.DeliveryItems.Remove(order);
            await dbContext.SaveChangesAsync();
        }
    }
}
