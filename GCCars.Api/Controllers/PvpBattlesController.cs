using System.Linq;
using GCCars.Application.Dto;
using GCCars.Application.Dto.PvpBattles;
using GCCars.Application.Extensions;
using GCCars.Application.Filters;
using GCCars.Application.Models.PvpBattles;
using GCCars.Application.Services;
using GCCars.Application.Validators.PvpBattles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GCCars.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PvpBattlesController : Controller
    {
        private readonly PvpBattlesService battlesService;
        private readonly CarsService carsService;
        private readonly PvpBattleJoinValidator battleJoinValidator;
        private readonly PvpBattleLeaveValidator battleLeaveValidator;
        private readonly PvpBattleCreateValidator battleCreateValidator;

        public PvpBattlesController(
            PvpBattlesService battlesService,
            CarsService carsService,
            PvpBattleJoinValidator battleJoinValidator,
            PvpBattleLeaveValidator battleLeaveValidator,
            PvpBattleCreateValidator battleCreateValidator)
        {
            this.battlesService = battlesService;
            this.carsService = carsService;
            this.battleJoinValidator = battleJoinValidator;
            this.battleLeaveValidator = battleLeaveValidator;
            this.battleCreateValidator = battleCreateValidator;
        }

        /// <summary>
        /// Получение списка организованных, но не начатых сражений.
        /// </summary>
        /// <param name="filter">Параметры пагинации и сортировки.</param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<ServerResponse<PagedData<PvpBattleDto>>> GetList([FromBody] BaseFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            var result = await battlesService.GetUnstartedBattles(userId.Value, filter.Skip, filter.Take, filter.Sorting).ConfigureAwait(false);
            return new ServerResponse<PagedData<PvpBattleDto>> { Data = result };
        }

        /// <summary>
        /// Получение данных сражения по его идентификатору.
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <returns></returns>
        [HttpGet("{battleId:int}")]
        public async Task<ServerResponse<PvpBattleDto>> Get(int battleId)
        {
            var result = await battlesService.GetBattle(battleId).ConfigureAwait(false);
            return new ServerResponse<PvpBattleDto> { Data = result };
        }

        /// <summary>
        /// Создание сражения.
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ServerResponse<int>> Create([FromBody] PvpBattleCreateModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            if (!await battleCreateValidator.Validate(model, userId.Value).ConfigureAwait(false))
                return new ServerResponse<int>
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = battleCreateValidator.GetErrors()
                };

            var result = await battlesService.CreateBattle(userId.Value, model.CarId, model.Level, model.MaxFighters, model.BetAmount).ConfigureAwait(false);
            return new ServerResponse<int> { Data = result };
        }

        /// <summary>
        /// Вступление в сражение.
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <returns></returns>
        [HttpPost("{battleId:int}/join/{carId:int}")]
        public async Task<ServerResponse> Join(int battleId, int carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            if (!await battleJoinValidator.Validate(battleId, carId).ConfigureAwait(false))
                return new ServerResponse
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = battleJoinValidator.GetErrors()
                };

            await battlesService.JoinBattle(userId.Value, battleId, carId).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Выход из сражения.
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <returns></returns>
        [HttpPost("{battleId:int}/leave")]
        public async Task<ServerResponse> Leave(int battleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            if (!await battleLeaveValidator.Validate(battleId, userId).ConfigureAwait(false))
                return new ServerResponse
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = battleLeaveValidator.GetErrors()
                };

            await battlesService.LeaveBattle(battleId, userId.Value).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Получение списка машинок для сражений.
        /// </summary>
        /// <param name="filter">Параметры отбора списка.</param>
        /// <returns>Список неповреждённых машинок.</returns>
        [HttpPost("cars/{battleId:int?}")]
        public async Task<ServerResponse<PagedData<CarFighterDto>>> GetCars(int? battleId, [FromBody] BaseFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            var result = await carsService.GetReadyForPvP(userId, battleId, filter.Skip, filter.Take, filter.Sorting).ConfigureAwait(false);
            return new ServerResponse<PagedData<CarFighterDto>> { Data = result };
        }
    }
}
