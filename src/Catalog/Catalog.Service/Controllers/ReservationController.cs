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

        [HttpPost("MakeOrder")]
        public async Task MakeOrder(IEnumerable<OrderItem> orderItems)
            => await reservationService.Reserve(orderItems);
        

        [HttpGet("GetItems/{orderId}")]
        public async Task<IEnumerable<Product>> GetItems(Guid orderId)
            => await reservationService.GetOrderProducts(orderId);

        [HttpPost("CancelOrder/{orderId}")]
        public async Task CancelOrder(Guid orderId)
            => await reservationService.CancelReservation(orderId);

        [HttpPost("FinishOrder/{orderId}")]
        public async Task FinishOrder(Guid orderId)
            => await reservationService.FinishOrder(orderId);

        [HttpGet("GetAllItems")]
        public async Task<IEnumerable<OrderItem>> GetAll()
            => await reservationService.GetAllItems();
    }
}
