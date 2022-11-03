using Models;
using Models.Enums;

namespace Order.DataAccess.Abstraction
{
    public interface IOrderRepository
    {
        Task AddOrder(OrderModel order);
        Task AddItemToOrder(OrderItem item);
        Task<IEnumerable<OrderItem>> GetOrderItems(Guid orderId);
        Task ChangeOrderStatus(Guid orderId, OrderStatus status);
        Task<OrderModel?> GetOrder(Guid orderId);
        Task<OrderItem?> GetItem(long itemId, Guid orderId);
        Task UpdateItemCount(long itemId, int count);
    }
}
