using Catalog.Core.Abstraction;
using Catalog.DataAccess.Abstraction;
using Catalog.DataAccess.Entities;
using Models;


namespace Catalog.Core.Service
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository repo;

        public CatalogService(ICatalogRepository repo)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task AddBrand(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && !await repo.IsBrandExists(name))
                await repo.AddBrand(new Brand(name));
        }

        public async Task AddCategory(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && !await repo.IsCategoryExists(name))
                await repo.AddCategory(new Category(name));
        }

        public async Task AddProduct(Product product)
        {
            if (product is null || !product.IsFull())
                return;

            if (await repo.IsValidProduct(product))
                await repo.AddProduct(product);
        }

        public async Task<bool> Exists(long id, int count)
        {
            var product = await repo.GetProduct(id);
            if (product is null)
                return false;

            return product.Count >= count;
        }

        public async Task<IEnumerable<Brand>> GetBrands(int itemsCount, int page)
        {
            if (itemsCount < 0) itemsCount = 20;
            if (page < 0) page = 0;
            return await repo.GetBrands(itemsCount * page, itemsCount);
        }

        public async Task<IEnumerable<Product>> GetCatalog(int itemsCount, int page)
        {
            if (itemsCount < 0) itemsCount = 20;
            if (page < 0) page = 0;
            return await repo.GetCatalog(itemsCount * page, itemsCount);
        }
        public async Task<IEnumerable<Category>> GetCategories(int itemsCount, int page)
        {
            if (itemsCount < 0) itemsCount = 20;
            if (page < 0) page = 0;
            return await repo.GetCategories(itemsCount * page, itemsCount);
        }
    }
}
