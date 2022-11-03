

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
        #region devonly
        public async Task<IEnumerable<OrderItem>> GetAllItems()
            => await reserveRepository.GetAll();

        #endregion

        public async Task CancelReservation(Guid orderId)
        {
            var items = await reserveRepository.GetReserveProducts(orderId);
            if (items is null || !items.Any())
                return;
            await catalogRepository.ReturnProducts(items);
            await reserveRepository.RemoveItems(items);
        }

        public async Task<IEnumerable<Product>> GetOrderProducts(Guid orderId)
        {
            var orderItems = await reserveRepository.GetReserveProducts(orderId);
            var orderItemIds = from item in orderItems
                               select item.ProductId;

            return await catalogRepository.GetProductsById(orderItemIds);
        }

        public async Task Reserve(IEnumerable<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
                return;

            var isUpdated = await catalogRepository.UpdateProductCount(orderItems);
            if (isUpdated)
                await reserveRepository.MakeReservation(orderItems);
        }

        public async Task FinishOrder(Guid orderId)
        {
            var items = await reserveRepository.GetReserveProducts(orderId);
            if (items is null)
                return;

            await reserveRepository.RemoveItems(items);
        }
    }
}
