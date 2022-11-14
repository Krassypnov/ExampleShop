using Models;
using Order.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.DataAccess.Dto;

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

        [HttpGet]
        public async Task<IEnumerable<OrderItem>> GetOrderItems()
            => await orderService.GetOrderItems();

        [HttpGet("{orderId}/info")]
        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
            => await orderService.GetOrderInfo(orderId);

        [HttpPost("product")]
        public async Task AddProduct(long productId, int count)
        {
            await orderService.AddToOrder(productId, count);
        }

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

        [HttpPost("{id}/cancel")]
        public async Task CancelOrder(Guid id)
            => await orderService.CancelOrder(id);

        [HttpPost("{id}/finish")]
        public async Task FinishOrder(Guid id)
            => await orderService.FinishOrder(id);
    }
}
