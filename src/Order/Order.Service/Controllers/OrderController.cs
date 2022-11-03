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

        [HttpGet("Catalog")]
        public async Task<IEnumerable<Product>> GetCatalog(int itemCount, int page)
        {
            var products = await orderService.GetCatalog(itemCount, page);
            return products;
        }

        [HttpGet("OrderItems")]
        public async Task<IEnumerable<OrderItem>> GetOrderItems()
            => await orderService.GetOrderItems();

        [HttpGet("GetOrderInfo/{orderId}")]
        public async Task<OrderModel?> GetOrderInfo(Guid orderId)
            => await orderService.GetOrderInfo(orderId);

        [HttpPost("AddProduct")]
        public async Task AddProduct(long productId, int count)
        {
            await orderService.AddToCatalog(productId, count);
        }

        [HttpPost("ConfirmOrder")]
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

        [HttpPost("CancelOrder/{orderId}")]
        public async Task CancelOrder(Guid orderId)
            => await orderService.CancelOrder(orderId);

        [HttpPost("FinishOrder/{orderId}")]
        public async Task FinishOrder(Guid orderId)
            => await orderService.FinishOrder(orderId);
    }
}
