using Catalog.DataAccess.Abstraction;
using Catalog.DataAccess.Dto;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.DataAccess.Repo
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogContext dbContext;

        public CatalogRepository(CatalogContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddBrand(Brand brand)
        {
            if (brand is null)
                return;
            if (await IsBrandExists(brand.Name))
                return;
            await dbContext.Brands.AddAsync(brand);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddCategory(Category category)
        {
            if (category is null)
                return;
            if (await IsCategoryExists(category.Name))
                return;
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddProduct(Product product)
        {
            if (product is null)
                return;

            if (await IsProductExists(product.Name))
                return;
            if (!await IsBrandExists(product.Brand))
                return;
            if (!await IsCategoryExists(product.Category))
                return;
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Brand>> GetBrands(int itemCount, int page)
        {
            if (itemCount <= 0) itemCount = 20;
            if (page < 0) page = 0;

            return await dbContext.Brands.Skip(itemCount * page).Take(itemCount).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetCatalog(int itemCount = 20, int page = 0)
        {
            if (itemCount <= 0) itemCount = 20;
            if (page < 0) page = 0;

            return await dbContext.Products.Skip(itemCount * page).Take(itemCount).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategories(int itemCount, int page)
        {
            if (itemCount <= 0) itemCount = 20;
            if (page < 0) page = 0;

            return await dbContext.Categories.Skip(itemCount * page).Take(itemCount).ToListAsync();
        }

        public async Task<Product?> GetProduct(long id)
            => await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<Product>> GetProductsById(IEnumerable<long> productIds)
            => await dbContext.Products
                            .Where(Product => productIds.Contains(Product.Id))
                            .ToListAsync();



        public async Task<bool> IsBrandExists(string name)
            => await dbContext.Brands
                        .Where(x => x.Name == name)
                        .AnyAsync();

        public async Task<bool> IsCategoryExists(string name)
            => await dbContext.Categories
                        .Where(x => x.Name == name)
                        .AnyAsync();

        public async Task<bool> IsProductExists(string name)
            => await dbContext.Products
                        .Where(x => x.Name == name)
                        .AnyAsync();

        public async Task ReturnProducts(IEnumerable<OrderItem> items)
        {
            if (items is null || !items.Any())
                return;

            foreach (var item in items)
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);
                if (product is null)
                    continue;
                product.Count += item.Count;

                var entry = dbContext.Entry(product);
                entry.Property(x => x.Count).IsModified = true;

            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateProductCount(IEnumerable<OrderItem> items)
        {
            if (items is null || !items.Any())
                return false;

            foreach (var item in items)
            {
                var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);

                if (product is null || item is null)
                    return false;

                var isCorrectCount = product.Count - item.Count >= 0 ? true : false;

                if (!isCorrectCount)
                    return false;

                product.Count -= item.Count;
                var entry = dbContext.Entry(product);
                entry.Property(x => x.Count).IsModified = true;
            }
           

            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
