using Catalog.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Catalog.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService reservationService;

        public ReservationController(IReservationService reservationService)
        {
            this.reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
        }

        /// <summary>
        /// Зарезервировать список продуктов
        /// </summary>
        /// <param name="orderItems">Список продуктов</param>
        /// <returns></returns>
        [HttpPost]
        public async Task Reserve(IEnumerable<OrderItem> orderItems)
            => await reservationService.Reserve(orderItems);
        
        /// <summary>
        /// Получить список зарезервированных продуктов в заказе
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns></returns>
        [HttpGet("order/{id}")]
        public async Task<IEnumerable<Product>> GetOrderItems(Guid orderId)
            => await reservationService.GetOrderProducts(orderId);

        /// <summary>
        /// Отменить резервацию продуктов
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns></returns>
        [HttpPost("order/{id}/cancel")]
        public async Task CancelOrder(Guid orderId)
            => await reservationService.CancelReservation(orderId);

        /// <summary>
        /// Завершить заказ
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns></returns>
        [HttpPost("order/{id}/finish")]
        public async Task FinishOrder(Guid orderId)
            => await reservationService.FinishOrder(orderId);

        /// <summary>
        /// Получить все зарезервированные продукты (dev only)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<OrderItem>> GetAll()
            => await reservationService.GetAllItems();
    }
}
