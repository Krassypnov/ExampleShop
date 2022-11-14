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
            await dbContext.Brands.AddAsync(brand);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddCategory(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
        }

        public async Task AddProduct(Product product)
        {
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Brand>> GetBrands(int skip, int take)
            => await dbContext.Brands.Skip(skip).Take(take).ToListAsync();
        

        public async Task<IEnumerable<Product>> GetCatalog(int skip, int take)
            => await dbContext.Products.Skip(skip).Take(take).ToListAsync();
        
        public async Task<IEnumerable<Category>> GetCategories(int skip, int take)
            => await dbContext.Categories.Skip(skip).Take(take).ToListAsync();
        

        public async Task<Product?> GetProduct(long id)
            => await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<Product>> GetProductsById(IEnumerable<long> productIds)
            => await dbContext.Products
                            .AsNoTracking()
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

        public async Task<bool> IsValidProduct(Product product)
        {
            return !(await IsProductExists(product.Name))
                 && (await IsBrandExists(product.Brand))
                 && (await IsCategoryExists(product.Category));
        }
    }
}
