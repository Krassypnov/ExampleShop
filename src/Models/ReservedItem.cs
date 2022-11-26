
namespace Models
{
    public class ReservedItem
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public int Count { get; set; }

        public ReservedItem(long productId, int count)
        {
            ProductId = productId;
            Count = count;
        }
    }
}
