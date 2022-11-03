﻿using Models;
using Order.Core.Message;
using Order.DataAccess.Dto;

namespace Order.Core.Abstraction
{
    public interface IOrderService
    {
        Task<IEnumerable<Product>> GetCatalog(int itemCount, int page);
        Task AddToCatalog(long productId, int count);
        Task<IEnumerable<OrderItem>> GetOrderItems();
        Task<OrderModel?> GetOrderInfo(Guid orderId);
        Task<ServiceResult> ConfirmOrder(ClientModel clientInfo);
        Task CancelOrder(Guid orderId);
        Task FinishOrder(Guid orderId); 
    }
}