
namespace Catalog.DataAccess.Entities
{
    public class Brand
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Brand(string name)
        {
            Name = name;
        }
    }
}
