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

        [HttpPost("[action]")]
        public async Task Reserve(IEnumerable<OrderItem> orderItems)
            => await reservationService.Reserve(orderItems);
        

        [HttpGet("order/{id}")]
        public async Task<IEnumerable<Product>> GetOrderItems(Guid orderId)
            => await reservationService.GetOrderProducts(orderId);

        [HttpPost("order/{id}/cancel")]
        public async Task CancelOrder(Guid orderId)
            => await reservationService.CancelReservation(orderId);

        [HttpPost("order/{id}/finish")]
        public async Task FinishOrder(Guid orderId)
            => await reservationService.FinishOrder(orderId);

        [HttpGet]
        public async Task<IEnumerable<OrderItem>> GetAll()
            => await reservationService.GetAllItems();
    }
}
