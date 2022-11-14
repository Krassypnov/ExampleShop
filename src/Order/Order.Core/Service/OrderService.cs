using Microsoft.AspNetCore.Http;
using Order.Core.Abstraction;
using Order.DataAccess.Abstraction;
using Models;
using Models.Enums;
using System.Net.Http.Json;
using Order.Core.Message;
using Order.DataAccess.Dto;

namespace Order.Core.Service
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory httpFactory;
        private readonly HttpContext httpContext;
        private readonly IOrderRepository orderRepository;
        private const string ORDER_ID_SESSSION = "CurrentOrderId";

        public OrderService(IHttpClientFactory httpFactory, IHttpContextAccessor httpContext, IOrderRepository orderRepository)
        {
            this.httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            this.httpContext = httpContext.HttpContext ?? throw new ArgumentNullException(nameof(httpContext));
            this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task AddToCatalog(long productId, int count = 1)
        {
            var isExists = await IsProductExists(productId, count);

            if (!isExists)
                return;

            var orderId = GetCurrentOrderId();

            var orderItem = await orderRepository.GetItem(productId, orderId);
            if (orderItem is null)
            {
                var item = new OrderItem(orderId, productId, count);
                await orderRepository.AddItemToOrder(item);
            }
            else
            {
                var newCount = orderItem.Count + count;
                var isCorrectCount = await IsProductExists(productId, newCount);
                if (isCorrectCount)
                    await orderRepository.UpdateItemCount(productId, newCount);
            }

        }

        public async Task<IEnumerable<Product>> GetCatalog(int itemCount, int page)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Catalog/GetCatalog/{itemCount},{page}");

            var client = httpFactory.CreateClient("CatalogService");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var catalog = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
                if (catalog is null)
                    return Enumerable.Empty<Product>();
                return catalog;
            }
            return Enumerable.Empty<Product>();
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItems()
            => await orderRepository.GetOrderItems(GetCurrentOrderId());
        

        public async Task<ServiceResult> ConfirmOrder(ClientModel clientInfo)
        {
            if (clientInfo is null)
                return new ServiceResult("Model is null", StatusCodes.Status400BadRequest);
            var orderId = GetCurrentOrderId();
            var orderItems = await GetOrderItems();
            if (orderItems is null)
                return new ServiceResult("Order has no items", StatusCodes.Status400BadRequest);

            var order = new OrderModel
            {
                Id = orderId,
                ClientName = clientInfo.Name,
                Phone = clientInfo.Phone,
                Address = clientInfo.Address,
                Delivery = clientInfo.Delivery,
                OrderStatus = OrderStatus.Created,
                CreatedTime = DateTime.UtcNow
            };

            await orderRepository.AddOrder(order);

            // Sending to CatalogService
            var catalogRequest = "api/Reservation/MakeOrder";
            var catalogService = httpFactory.CreateClient("CatalogService");
            await catalogService.PostAsJsonAsync(catalogRequest, orderItems);

            // Sending to DeliveryService
            if (order.Delivery)
            {
                var deliveryRequest = $"api/Delivery/HandleOrder/{order.Id}";
                var deliveryService = httpFactory.CreateClient("DeliveryService");
                await deliveryService.PostAsync(deliveryRequest, null);
            }
            

            ClearOrderId();
            return new ServiceResult(orderId.ToString(), StatusCodes.Status200OK);
        }

        public async Task CancelOrder(Guid orderId)
        {
            var catalogRequest = $"api/Reservation/CancelOrder/{orderId}";
            var catalogService = httpFactory.CreateClient("CatalogService");
            await catalogService.PostAsync(catalogRequest, null);
            await orderRepository.ChangeOrderStatus(orderId, OrderStatus.Canceled);
        }

        public async Task FinishOrder(Guid orderId)
        {
            var catalogRequest = $"api/Reservation/FinishOrder/{orderId}";
            var catalogService = httpFactory.CreateClient("CatalogService");
            await catalogService.PostAsync(catalogRequest, null);
            await orderRepository.ChangeOrderStatus(orderId, OrderStatus.Delivered);
        }

        private async Task<bool> IsProductExists(long id, int count)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Catalog/Exists/{id},{count}");
            var client = httpFactory.CreateClient("CatalogService");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var isExists = await response.Content.ReadFromJsonAsync<bool>();
                return isExists;
            }
            return false;
        }

        private Guid GetCurrentOrderId()
        {
            if (httpContext.Session.Keys.Contains(ORDER_ID_SESSSION))
                return new Guid(httpContext.Session.GetString(ORDER_ID_SESSSION));

            var currentId = Guid.NewGuid();
            httpContext.Session.SetString(ORDER_ID_SESSSION, currentId.ToString());
            return currentId;
        }

        private void ClearOrderId()
        {
            if (httpContext.Session.Keys.Contains(ORDER_ID_SESSSION))
                httpContext.Session.Remove(ORDER_ID_SESSSION);
        }

        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
            => await orderRepository.GetOrder(orderId);
    }
}
