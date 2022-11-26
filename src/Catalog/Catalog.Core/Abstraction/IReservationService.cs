

using Models;

namespace Catalog.Core.Abstraction
{
    public interface IReservationService
    {
        Task AddOrUpdate(IEnumerable<ReservedItem> items);
        Task RemoveOrUpdate(IEnumerable<ReservedItem> items);
        Task Return(IEnumerable<ReservedItem> items);
        Task<IEnumerable<ReservedItem>> GetReservedItems(int page, int pageSize);
    }
}
