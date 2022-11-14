using Delivery.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Delivery.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            this.deliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));
        }

        /// <summary>
        /// Получить список продуктов в заказе
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpGet("order/{id}/products")]
        public async Task<IEnumerable<Product>?> GetProducts(Guid id)
            => await deliveryService.GetProducts(id);

        /// <summary>
        /// Получить список всех заказов
        /// </summary>
        /// <returns></returns>
        [HttpGet("orders")]
        public async Task<IEnumerable<DeliveryItem>> GetAllOrders()
            => await deliveryService.GetAllOrders();

        /// <summary>
        /// Получить информацию о заказе
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpGet("order/{id}/info")]
        public async Task<OrderModel?> GetOrderInfo(Guid id)
            => await deliveryService.GetOrderInfo(id);

        /// <summary>
        /// Вернуть заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpPost("order/{id}/return")]
        public async Task ReturnOrder(Guid id)
            => await deliveryService.ReturnOrder(id);

        /// <summary>
        /// Завершить заказ (доставить)
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpPost("order/{id}/finish")]
        public async Task FinishOrder(Guid id)
            => await deliveryService.FinishOrder(id);

        /// <summary>
        /// Добавить заказ для доставки
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpPost("order/{id}")]
        public async Task HandleOrder(Guid id)
            => await deliveryService.RecordOrder(id);
    }
}
