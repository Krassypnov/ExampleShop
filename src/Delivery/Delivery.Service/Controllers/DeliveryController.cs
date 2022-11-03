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
        [HttpGet("GetOrderProducts/{orderId}")]
        public async Task<IEnumerable<Product>?> GetProducts(Guid orderId)
            => await deliveryService.GetProducts(orderId);

        [HttpGet("GetAllOrders")]
        public async Task<IEnumerable<DeliveryItem>> GetAllOrders()
            => await deliveryService.GetAllOrders();

        [HttpGet("GetOrderInfo/{orderId}")]
        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
            => await deliveryService.GetOrderInfo(orderId);

        [HttpPost("ReturnOrder/{orderId}")]
        public async Task ReturnOrder(Guid orderId)
            => await deliveryService.ReturnOrder(orderId);

        [HttpPost("FinishOrder/{orderId}")]
        public async Task FinishOrder(Guid orderId)
            => await deliveryService.FinishOrder(orderId);

        [HttpPost("HandleOrder/{orderId}")]
        public async Task HandleOrder(Guid orderId)
            => await deliveryService.RecordOrder(orderId);
    }
}
