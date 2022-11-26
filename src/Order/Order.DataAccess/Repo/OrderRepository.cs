using Microsoft.EntityFrameworkCore;
using Order.DataAccess.Abstraction;
using Models;
using Models.Enums;

namespace Order.DataAccess.Repo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext dbContext;

        public OrderRepository(OrderContext orderContext)
        {
            this.dbContext = orderContext ?? throw new ArgumentNullException(nameof(orderContext));
        }

        public async Task AddOrder(OrderModel order)
        {
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddItemToOrder(OrderItem item)
        {
            await dbContext.OrderItems.AddAsync(item);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItems(Guid orderId)
            => await dbContext.OrderItems
                                 .AsNoTracking()
                                 .Where(x => x.OrderId == orderId)
                                 .ToListAsync();

        public async Task<OrderModel?> GetOrder(Guid orderId)
            => await dbContext.Orders
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.Id == orderId);

        public async Task<OrderItem?> GetItem(long itemId, Guid orderId)
            => await dbContext.OrderItems
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(x => x.ProductId == itemId && x.OrderId == orderId);

        public async Task UpdateItemCount(long itemId, int count)
        {
            var item = await dbContext.OrderItems.FirstOrDefaultAsync(x => x.ProductId == itemId);
            if (item is null)
                return;
            item.Count += count;
            var entry = dbContext.Entry(item);
            entry.Property(x => x.Count).IsModified = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task ChangeOrderStatus(Guid orderId, OrderStatus status)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is null)
                return;

            order.OrderStatus = status;
            var entry = dbContext.Entry(order);
            entry.Property(x => x.OrderStatus).IsModified = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task<OrderStatus> GetStatus(Guid orderId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            return order == null ? OrderStatus.Undefined : order.OrderStatus;
        }
    }
}
