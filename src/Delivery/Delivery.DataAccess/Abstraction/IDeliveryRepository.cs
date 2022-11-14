using Models;

namespace Delivery.DataAccess.Abstraction
{
    public interface IDeliveryRepository
    {
        Task<IEnumerable<DeliveryItem>> GetAllOrders();
        Task<DeliveryItem?> GetOrder(Guid orderId);
        Task RecordOrder(DeliveryItem item);
        Task RemoveOrder(Guid orderId);
    }
}
