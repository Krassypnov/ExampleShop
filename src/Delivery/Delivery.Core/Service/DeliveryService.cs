

using Delivery.Core.Abstraction;
using Delivery.DataAccess.Abstraction;
using Delivery.DataAccess.Dto;
using Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Delivery.Core.Service
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IHttpClientFactory httpFactory;
        private readonly IDeliveryRepository deliveryRepository;

        public DeliveryService(IHttpClientFactory httpFactory, IDeliveryRepository deliveryRepository)
        {
            this.httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            this.deliveryRepository = deliveryRepository ?? throw new ArgumentNullException(nameof(deliveryRepository));
        }

        public async Task<IEnumerable<DeliveryItem>> GetAllOrders()
            => await deliveryRepository.GetAllOrders();

        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
        {
            var orderRequest = $"api/Order/{orderId}/info";
            var orderService = httpFactory.CreateClient("OrderService");
            var orderInfo = await orderService.GetFromJsonAsync<OrderModel>(orderRequest);

            return orderInfo;
        }

        public async Task<IEnumerable<Product>?> GetProducts(Guid orderId)
        {
            var catalogRequest = $"api/Reservation/order/{orderId}";
            var catalogService = httpFactory.CreateClient("CatalogService");
            var products = await catalogService.GetFromJsonAsync<IEnumerable<Product>>(catalogRequest);

            return products;
        }

        public async Task RecordOrder(Guid orderId)
            => await deliveryRepository.RecordOrder(new DeliveryItem(orderId, DateTime.UtcNow));

        public async Task ReturnOrder(Guid orderId)
        {
            var catalogRequest = $"api/Order/{orderId}/cancel";
            var catalogService = httpFactory.CreateClient("OrderService");
            var response = await catalogService.PostAsync(catalogRequest, null);

            if (response.IsSuccessStatusCode)
                await deliveryRepository.RemoveOrder(orderId);
        }

        public async Task FinishOrder(Guid orderId)
        {
            var orderRequest = $"api/Order/{orderId}/finish";
            var orderService = httpFactory.CreateClient("OrderService");
            var response = await orderService.PostAsync(orderRequest, null);

            if (response.IsSuccessStatusCode)
                await deliveryRepository.RemoveOrder(orderId);
        }
    }
}
