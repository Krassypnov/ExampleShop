using Catalog.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Catalog.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService reservationService;

        public ReservationController(IReservationService reservationService)
        {
            this.reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
        }

        /// <summary>
        /// Зарезервировать продукты
        /// </summary>
        /// <param name="items">Список продуктов</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task Add(IEnumerable<ReservedItem> items)
            => await reservationService.AddOrUpdate(items);

        /// <summary>
        /// Убрать зарезервированные продукты (когда товар реализован)
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task Remove(IEnumerable<ReservedItem> items)
            => await reservationService.RemoveOrUpdate(items);

        /// <summary>
        /// Отменить и вернуть продукты 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task Return(IEnumerable<ReservedItem> items)
            => await reservationService.Return(items);

        /// <summary>
        /// Получить зарезервированные продукты
        /// </summary>
        /// <param name="page">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IEnumerable<ReservedItem>> Get(int page = 0, int pageSize = 20)
            => await reservationService.GetReservedItems(page, pageSize);
    }
}
