

using Models;

namespace Catalog.DataAccess.Abstraction
{
    public interface IReserveRepository
    {
        Task AddOrUpdate(IEnumerable<ReservedItem> items);
        Task RemoveOrUpdate(IEnumerable<ReservedItem> items);
        Task<IEnumerable<ReservedItem>> GetItems(int skip, int take);
    }
}
