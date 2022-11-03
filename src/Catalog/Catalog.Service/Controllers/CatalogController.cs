using Catalog.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Catalog.DataAccess.Dto;
using Models;

namespace Catalog.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        }

        [HttpGet("GetCatalog/{itemCount},{page}")]
        public async Task<IEnumerable<Product>> GetCatalog(int itemCount = 20, int page = 0)
            => await catalogService.GetCatalog(itemCount, page);

        [HttpGet("Brand")]
        public async Task<IEnumerable<Brand>> GetBrands(int itemCount = 20, int page = 0)
            => await catalogService.GetBrands(itemCount, page);

        [HttpGet("Category")]
        public async Task<IEnumerable<Category>> GetCategories(int itemCount = 20, int page = 0)
            => await catalogService.GetCategories(itemCount, page);

        [HttpPost("Product")]
        public async Task AddProduct(Product product)
        {
            await catalogService.AddProduct(product);
        }

        [HttpPost("Brand")]
        public async Task AddBrand(string name)
        {
            await catalogService.AddBrand(name);
        }

        [HttpPost("Category")]
        public async Task AddCategory(string name)
        {
            await catalogService.AddCategory(name);
        }

        [HttpGet("Exists/{productId},{count}")]
        public async Task<bool> IsProductExists(long productId, int count)
            => await catalogService.Exists(productId, count);
    }
}
