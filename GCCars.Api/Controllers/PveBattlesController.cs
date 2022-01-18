using GCCars.Application.Dto;
using GCCars.Application.Dto.Cars;
using GCCars.Application.Dto.PveBattles;
using GCCars.Application.Extensions;
using GCCars.Application.Filters;
using GCCars.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GCCars.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PveBattlesController : Controller
    {
        public readonly CarsService carsService;
        public readonly PveBattlesService battlesService;

        public PveBattlesController(
            CarsService carsService,
            PveBattlesService battlesService)
        {
            this.carsService = carsService;
            this.battlesService = battlesService;
        }

        /// <summary>
        /// Получение списка машинок пользователя для PvE сражения.
        /// </summary>
        /// <param name="filter">Параметры отбора и сортировки.</param>
        /// <returns></returns>
        [HttpPost("cars")]
        public async Task<ServerResponse<PagedData<CarDto>>> GetCars([FromBody] BaseFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            var result = await carsService.GetReadyForPvE(userId.Value, filter.Skip, filter.Take, filter.Sorting).ConfigureAwait(false);
            return new ServerResponse<PagedData<CarDto>> { Data = result };
        }

        /// <summary>
        /// Начало PvE сражения.
        /// </summary>
        /// <param name="carId">Идентификатор машинки в сражении.</param>
        /// <returns>Результаты сражения.</returns>
        [HttpPost("start/{carId:int}")]
        public async Task<ServerResponse<PveBattleResultDto>> Start(int carId)
        {
            var result = await battlesService.StartAndGetResult(carId).ConfigureAwait(false);
            return new ServerResponse<PveBattleResultDto> { Data = result };
        }

        /// <summary>
        /// Завершение PvE сражения (подтверждение получения результата).
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <returns></returns>
        [HttpPost("complete/{battleId:int}")]
        public async Task<ServerResponse> Complete(int battleId)
        {
            await battlesService.CompleteBattle(battleId).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Получение списка незавершенных PvE сражений пользователя для вывода модалки.
        /// </summary>
        /// <returns></returns>
        [HttpGet("uncompleted")]
        public async Task<ServerResponse<PveBattleResultDto[]>> GetUncompleted()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            var result = await battlesService.GetUncompleted(userId.Value).ConfigureAwait(false);
            return new ServerResponse<PveBattleResultDto[]> { Data = result };
        }
    }
}
