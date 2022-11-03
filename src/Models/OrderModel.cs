using Models.Enums;


namespace Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public string? ClientName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Delivery { get; set; }
        public OrderStatus OrderStatus { get; set; }

    }
}
