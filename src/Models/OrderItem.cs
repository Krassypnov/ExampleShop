

namespace Models
{
    public class OrderItem
    {
        public long Id { get; set; }
        public Guid OrderId { get; set; }
        public long ProductId { get; set; }
        public int Count { get; set; }

        public OrderItem(Guid orderId, long productId, int count)
        {
            OrderId = orderId;
            ProductId = productId;
            Count = count;
        }
    }
}
