
namespace Models
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

        public Product(string name, string brand, string category, int count, decimal price)
        {
            Name = name;
            Brand = brand;
            Category = category;
            Count = count;
            Price = price;
        }

        public bool IsFull()
            => !(string.IsNullOrWhiteSpace(Name)
            || string.IsNullOrWhiteSpace(Brand)
            || string.IsNullOrWhiteSpace(Category));
    }
}
