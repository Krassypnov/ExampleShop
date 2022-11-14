using Microsoft.AspNetCore.Http;
using Order.Core.Abstraction;
using Order.DataAccess.Abstraction;
using Models;
using Models.Enums;
using System.Net.Http.Json;
using Order.Core.Message;
using Order.DataAccess.Dto;
using System.Net.Http.Headers;

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

        public async Task AddToOrder(long productId, int count = 1)
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
            var catalogRequest = "api/Reservation/Reserve";
            var catalogService = httpFactory.CreateClient("CatalogService");
            await catalogService.PostAsJsonAsync(catalogRequest, orderItems);

            // Sending to DeliveryService
            if (order.Delivery)
            {
                var deliveryRequest = $"api/Delivery/order/{order.Id}";
                await PostAsync("DeliveryService", deliveryRequest);
            }
            

            ClearOrderId();
            return new ServiceResult(orderId.ToString(), StatusCodes.Status200OK);
        }

        public async Task CancelOrder(Guid orderId)
        {
            var catalogRequest = $"api/Reservation/order/{orderId}/cancel";
            await PostAsync("CatalogService", catalogRequest);
            await orderRepository.ChangeOrderStatus(orderId, OrderStatus.Canceled);
        }

        public async Task FinishOrder(Guid orderId)
        {
            var catalogRequest = $"api/Reservation/order/{orderId}/finish";
            await PostAsync("CatalogService", catalogRequest);
            await orderRepository.ChangeOrderStatus(orderId, OrderStatus.Delivered);
        }

        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
            => await orderRepository.GetOrder(orderId);

        private async Task<bool> IsProductExists(long id, int count)
        {
            var requestUri = $"api/Catalog/product/{id}/exists";
            var content = new StringContent(count.ToString());
            var serviceName = "CatalogService";
            var response = await GetAsync(serviceName, requestUri, content);
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

        private async Task<HttpResponseMessage> GetAsync(string serviceName, string requestUri, StringContent content = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (content is not null)
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = content;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            var client = httpFactory.CreateClient(serviceName);
            return await client.SendAsync(request);
        }

        private async Task PostAsync(string serviceName, string requestUri)
        {
            var service = httpFactory.CreateClient(serviceName);
            await service.PostAsync(requestUri, null);
        }

    }
}
