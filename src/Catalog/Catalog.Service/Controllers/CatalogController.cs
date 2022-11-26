using Catalog.Core.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Catalog.DataAccess.Entities;
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

        /// <summary>
        /// Получить список продуктов в каталоге
        /// </summary>
        /// <param name="itemCount">Размер страницы</param>
        /// <param name="page">Номер страницы</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Product>> GetCatalog(int itemCount = 20, int page = 0)
            => await catalogService.GetCatalog(itemCount, page);

        /// <summary>
        /// Получить список брендов
        /// </summary>
        /// <param name="itemCount">Размер страницы</param>
        /// <param name="page">Номер страницы</param>
        /// <returns></returns>
        [HttpGet("brands")]
        public async Task<IEnumerable<Brand>> GetBrands(int itemCount = 20, int page = 0)
            => await catalogService.GetBrands(itemCount, page);

        /// <summary>
        /// Получить список категорий
        /// </summary>
        /// <param name="itemCount">Размер страницы</param>
        /// <param name="page">Номер страницы</param>
        /// <returns></returns>
        [HttpGet("categories")]
        public async Task<IEnumerable<Category>> GetCategories(int itemCount = 20, int page = 0)
            => await catalogService.GetCategories(itemCount, page);

        /// <summary>
        /// Добавить продукт в каталог
        /// </summary>
        /// <param name="product">Модель продукта</param>
        /// <returns></returns>
        [HttpPost("product")]
        public async Task AddProduct(Product product)
        {
            await catalogService.AddProduct(product);
        }

        /// <summary>
        /// Добавить бренд
        /// </summary>
        /// <param name="name">Название бренда</param>
        /// <returns></returns>
        [HttpPost("brand")]
        public async Task AddBrand(string name)
        {
            await catalogService.AddBrand(name);
        }

        /// <summary>
        /// Добавить категорию
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <returns></returns>
        [HttpPost("category")]
        public async Task AddCategory(string name)
        {
            await catalogService.AddCategory(name);
        }

        /// <summary>
        /// Проверить существует ли продукт в каталоге в необходимом количестве
        /// </summary>
        /// <param name="id">ID продукта</param>
        /// <param name="count">Количество</param>
        /// <returns></returns>
        [HttpGet("product/{id}/exists")]
        public async Task<bool> IsProductExists(long id,[FromBody] int count)
            => await catalogService.Exists(id, count);

        /// <summary>
        /// Получить список продуктов
        /// </summary>
        /// <param name="productIds">Список ID продуктов</param>
        /// <returns></returns>
        [HttpGet("products/info")]
        public async Task<IEnumerable<Product>> GetProductsById([FromBody] IEnumerable<long> productIds)
            => await catalogService.GetProductsById(productIds);
    }
}
