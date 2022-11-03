

using Models;

namespace Catalog.Core.Abstraction
{
    public interface IReservationService
    {
        Task Reserve(IEnumerable<OrderItem> orderItems);
        Task<IEnumerable<Product>> GetOrderProducts(Guid orderId);
        Task CancelReservation(Guid orderId); 
        Task FinishOrder(Guid orderId);
        Task<IEnumerable<OrderItem>> GetAllItems();
    }
}
