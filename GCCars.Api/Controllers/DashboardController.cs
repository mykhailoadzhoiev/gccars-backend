using GCCars.Application.Dto;
using GCCars.Application.Dto.Cars;
using GCCars.Application.Dto.Dashboard;
using GCCars.Application.Filters;
using GCCars.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GCCars.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DashboardService dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        /// <summary>
        /// Получить итоговые суммы продаж.
        /// </summary>
        /// <returns></returns>
        [HttpGet("totals")]
        public async Task<ServerResponse<TotalsDto>> GetTotals()
        {
            var result = await dashboardService.GetTotals().ConfigureAwait(false);
            return new ServerResponse<TotalsDto> { Data = result };
        }

        /// <summary>
        /// Получение списка загруженных в платформу машинок.
        /// </summary>
        /// <param name="filter">Пагинация и сортировка (по умолчанию последние в начале).</param>
        /// <returns></returns>
        [HttpPost("put-into")]
        public async Task<ServerResponse<PagedData<CarListItemDto>>> GetPutInto([FromBody] BaseFilter filter)
        {
            var result = await dashboardService.GetPutIntoCars(filter.Skip, filter.Take, filter.Sorting).ConfigureAwait(false);
            return new ServerResponse<PagedData<CarListItemDto>> { Data = result };
        }

        /// <summary>
        /// Получение списка проданных машинок.
        /// </summary>
        /// <param name="filter">Пагинация и сортировка (по умолчанию последние в начале).</param>
        /// <returns></returns>
        [HttpPost("recently-sold")]
        public async Task<ServerResponse<PagedData<CarListItemDto>>> GetRecentlySold([FromBody] BaseFilter filter)
        {
            var result = await dashboardService.GetRecentlySoldCars(filter.Skip, filter.Take, filter.Sorting).ConfigureAwait(false);
            return new ServerResponse<PagedData<CarListItemDto>> { Data = result };
        }
    }
}
