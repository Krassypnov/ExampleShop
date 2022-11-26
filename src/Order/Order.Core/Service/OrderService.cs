using Microsoft.AspNetCore.Http;
using Order.Core.Abstraction;
using Order.DataAccess.Abstraction;
using Models;
using Models.Enums;
using System.Net.Http.Json;
using Order.Core.Message;
using Order.Core.Entities;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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
            var orderId = GetCurrentOrderId();
            var isExists = await IsProductExists(productId, count);

            if (!isExists)
                return;

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
            var orderId = GetCurrentOrderId();
            if (await orderRepository.GetStatus(orderId) != OrderStatus.Undefined)
                return new ServiceResult("Order is inactive", StatusCodes.Status400BadRequest);
            if (clientInfo is null)
                return new ServiceResult("Model is null", StatusCodes.Status400BadRequest);
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
            var catalogRequest = "api/Reservation/Add";
            var catalogService = httpFactory.CreateClient("CatalogService");
            await catalogService.PostAsJsonAsync(catalogRequest, orderItems);

            // Sending to DeliveryService
            if (order.Delivery)
            {
                var deliveryRequest = $"api/Delivery/{order.Id}";
                await PostAsync("DeliveryService", deliveryRequest);
            }
            

            ClearOrderId();
            return new ServiceResult(orderId.ToString(), StatusCodes.Status200OK);
        }

        public async Task CancelOrder(Guid orderId)
        {
            if (!await IsActiveOrder(orderId))
                return;
            var catalogRequest = "api/Reservation/Return";
            var items = await orderRepository.GetOrderItems(orderId);
            var reservedItems = from item in items
                                select new ReservedItem(item.ProductId, item.Count);
            await PostJsonAsync("CatalogService", catalogRequest, reservedItems);
            await orderRepository.ChangeOrderStatus(orderId, OrderStatus.Canceled);
        }

        public async Task FinishOrder(Guid orderId)
        {
            if (!await IsActiveOrder(orderId))
                return;
            var catalogRequest = "api/Reservation/Remove";
            var items = await orderRepository.GetOrderItems(orderId);
            var reservedItems = from item in items
                                select new ReservedItem(item.ProductId, item.Count);
            await PostJsonAsync("CatalogService", catalogRequest, reservedItems);
            await orderRepository.ChangeOrderStatus(orderId, OrderStatus.Finished);
        }

        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
            => await orderRepository.GetOrder(orderId);

        public async Task<IEnumerable<Product>> GetProducts(Guid orderId)
        {
            var items = await orderRepository.GetOrderItems(orderId);
            var itemsIds = items.Select(x => x.ProductId);

            var requestUri = $"api/Catalog/products/info";
            var json = JsonConvert.SerializeObject(itemsIds, Formatting.Indented);
            var content = new StringContent(json);
            var serviceName = "CatalogService";

            var resposne = await GetAsync(serviceName, requestUri, content);
            if (resposne.IsSuccessStatusCode)
            {
                var products = await resposne.Content.ReadFromJsonAsync<IEnumerable<Product>>();
                if (products is not null)
                    return products;
            }
            return Enumerable.Empty<Product>();
        }

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

        private async Task PostJsonAsync<T>(string serviceName, string requestUri, T content)
        {
            var service = httpFactory.CreateClient(serviceName);
            await service.PostAsJsonAsync(requestUri, content);
        }

        private async Task PostAsync(string serviceName, string requestUri)
        {
            var service = httpFactory.CreateClient(serviceName);
            await service.PostAsync(requestUri, null);
        }

        private async Task<bool> IsActiveOrder(Guid orderId)
        {
            var status = await orderRepository.GetStatus(orderId);
            return status == OrderStatus.Created;
        }
    }
}
