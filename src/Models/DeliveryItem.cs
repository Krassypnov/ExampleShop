

namespace Models
{
    public class DeliveryItem
    {
        public long Id { get; set; }
        public Guid OrderId { get; set; }
        public DateTime CreatedDate { get; set; }

        public DeliveryItem(Guid orderId, DateTime createdDate)
        {
            OrderId = orderId;
            CreatedDate = createdDate;
        }
    }
}
