using Models;

namespace Delivery.Core.Abstraction
{
    public interface IDeliveryService
    {
        Task<IEnumerable<Product>?> GetProducts(Guid orderId);
        Task<IEnumerable<DeliveryItem>> GetAllOrders();
        Task<OrderModel?> GetOrderInfo(Guid orderId);
        Task RecordOrder(Guid orderId);
        Task ReturnOrder(Guid orderId);
        Task FinishOrder(Guid orderId);
    }
}
