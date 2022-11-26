using Models;
using Order.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Core.Entities;

namespace Order.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        /// <summary>
        /// Получить элементы заказа
        /// </summary>
        /// <returns>Список элементов заказа</returns>
        [HttpGet]
        public async Task<IEnumerable<OrderItem>> GetOrderItems()
            => await orderService.GetOrderItems();

        /// <summary>
        /// Получить информацию о заказе
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns></returns>
        [HttpGet("{orderId}/info")]
        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
            => await orderService.GetOrderInfo(orderId);

        /// <summary>
        /// Добавить продукт в заказ
        /// </summary>
        /// <param name="productId">ID продукта</param>
        /// <param name="count">Количество продуктов</param>
        /// <returns></returns>
        [HttpPost("product")]
        public async Task AddProduct(long productId, int count)
        {
            await orderService.AddToOrder(productId, count);
        }

        /// <summary>
        /// Подтвердить заказ
        /// </summary>
        /// <param name="clientInfo">Информация о клиенте</param>
        /// <returns></returns>
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder(ClientModel clientInfo)
        {
            if (clientInfo is null)
                return BadRequest("Parametr is null");

            if (!clientInfo.IsClientInfoFull())
                return BadRequest("Incorrect client information");

            var result = await orderService.ConfirmOrder(clientInfo);

            if (result.StatusCode == StatusCodes.Status200OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        /// <summary>
        /// Отменить заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpPost("{id}/cancel")]
        public async Task CancelOrder(Guid id)
            => await orderService.CancelOrder(id);

        /// <summary>
        /// Завершить заказ (получить)
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpPost("{id}/finish")]
        public async Task FinishOrder(Guid id)
            => await orderService.FinishOrder(id);

        /// <summary>
        /// Получить продукты в заказе
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns></returns>
        [HttpGet("{id}/products")]
        public async Task<IEnumerable<Product>> GetProducts(Guid id)
            => await orderService.GetProducts(id);
    }
}
