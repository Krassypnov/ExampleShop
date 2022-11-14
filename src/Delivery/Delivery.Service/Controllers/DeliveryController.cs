using Delivery.Core.Abstraction;
using Delivery.DataAccess.Dto;
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
        [HttpGet("order/{id}/products")]
        public async Task<IEnumerable<Product>?> GetProducts(Guid id)
            => await deliveryService.GetProducts(id);

        [HttpGet("orders")]
        public async Task<IEnumerable<DeliveryItem>> GetAllOrders()
            => await deliveryService.GetAllOrders();

        [HttpGet("order/{id}/info")]
        public async Task<OrderModel?> GetOrderInfo(Guid id)
            => await deliveryService.GetOrderInfo(id);

        [HttpPost("order/{id}/return")]
        public async Task ReturnOrder(Guid id)
            => await deliveryService.ReturnOrder(id);

        [HttpPost("order/{id}/finish")]
        public async Task FinishOrder(Guid id)
            => await deliveryService.FinishOrder(id);

        [HttpPost("order/{id}")]
        public async Task HandleOrder(Guid id)
            => await deliveryService.RecordOrder(id);
    }
}
