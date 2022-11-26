

using Catalog.Core.Abstraction;
using Catalog.DataAccess.Abstraction;
using Models;

namespace Catalog.Core.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IReserveRepository reserveRepository;
        private readonly ICatalogRepository catalogRepository;

        public ReservationService(IReserveRepository reserveRepository, ICatalogRepository catalogRepository)
        {
            this.reserveRepository = reserveRepository ?? throw new ArgumentNullException(nameof(reserveRepository));
            this.catalogRepository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
        }

        public async Task AddOrUpdate(IEnumerable<ReservedItem> items)
        {
            if (items is null || !items.Any())
                return;

            var isUpdated = await catalogRepository.UpdateProductCount(items);
            if (isUpdated)
                await reserveRepository.AddOrUpdate(items);
        }

        public async Task<IEnumerable<ReservedItem>> GetReservedItems(int page, int pageSize)
        {
            if (page < 0) page = 0;
            if (pageSize < 0 || pageSize > 1000) pageSize = 20;

            return await reserveRepository.GetItems(page * pageSize, pageSize);
        }

        public async Task RemoveOrUpdate(IEnumerable<ReservedItem> items)
        {
            if (items is null || !items.Any())
                return;

            await reserveRepository.RemoveOrUpdate(items);
        }

        public async Task Return(IEnumerable<ReservedItem> items)
        {
            if (items is null || !items.Any())
                return;

            await catalogRepository.ReturnProducts(items);
            await reserveRepository.RemoveOrUpdate(items);
        }
    }
}
