using Catalog.DataAccess.Dto;
using Models;

namespace Catalog.DataAccess.Abstraction
{
    public interface ICatalogRepository
    {
        Task AddProduct(Product product);
        Task AddBrand(Brand brand);
        Task AddCategory(Category category);
        Task<IEnumerable<Product>> GetCatalog(int skip, int take);
        Task<Product?> GetProduct(long id);
        Task<IEnumerable<Product>> GetProductsById(IEnumerable<long> productIds);
        Task<IEnumerable<Brand>> GetBrands(int skip, int take);
        Task<IEnumerable<Category>> GetCategories(int skip, int take);
        Task<bool> IsProductExists(string name);
        Task<bool> IsBrandExists(string name);
        Task<bool> IsCategoryExists(string name);
        Task<bool> IsValidProduct(Product product);
        Task<bool> UpdateProductCount(IEnumerable<OrderItem> items);
        Task ReturnProducts(IEnumerable<OrderItem> items);
    }
}
