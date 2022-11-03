

using Models;

namespace Catalog.DataAccess.Abstraction
{
    public interface IReserveRepository
    {
        Task MakeReservation(IEnumerable<OrderItem> orderItems);
        Task<IEnumerable<OrderItem>> GetReserveProducts(Guid orderId);
        Task RemoveItems(IEnumerable<OrderItem> orderItems);
        Task<IEnumerable<OrderItem>> GetAll();
    }
}
