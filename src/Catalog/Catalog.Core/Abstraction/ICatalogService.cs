using Catalog.DataAccess.Entities;
using Models;

namespace Catalog.Core.Abstraction
{
    public interface ICatalogService
    {
        Task<IEnumerable<Product>> GetCatalog(int itemsCount, int page);
        Task<IEnumerable<Brand>> GetBrands(int itemsCount, int page);
        Task<IEnumerable<Category>> GetCategories(int itemsCount, int page);
        Task<IEnumerable<Product>> GetProductsById(IEnumerable<long> productIds);
        Task AddProduct(Product product);
        Task AddBrand(string name);
        Task AddCategory(string name);
        Task<bool> Exists(long id, int count);
    }
}
