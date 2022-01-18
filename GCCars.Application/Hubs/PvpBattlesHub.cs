using GCCars.Application.Dto;
using GCCars.Application.Dto.PvpBattles;
using GCCars.Application.Extensions;
using GCCars.Application.Filters;
using GCCars.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace GCCars.Application.Hubs
{
    [Authorize]
    public class PvpBattlesHub : Hub
    {
        private readonly PvpBattlesService battlesService;

        public PvpBattlesHub(PvpBattlesService battlesService)
        {
            this.battlesService = battlesService;
        }

        /// <summary>
        /// Получение данных PvP сражения по его идентификатору.
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <returns></returns>
        public async Task<ServerResponse<PvpBattleDto>> GetPvpBattle(int battleId)
        {
            return new ServerResponse<PvpBattleDto> { Data = await battlesService.GetBattle(battleId).ConfigureAwait(false) };
        }

        /// <summary>
        /// Получение списка организованных, но не начатых сражений.
        /// </summary>
        /// <param name="filter">Параметры пагинации и сортировки.</param>
        /// <returns></returns>
        public async Task<ServerResponse<PagedData<PvpBattleDto>>> ListPvpBattles(BaseFilter filter)
        {
            var userId = Context.UserIdentifier.ToInt();
            var result = await battlesService.GetUnstartedBattles(userId.Value, filter.Skip, filter.Take, filter.Sorting).ConfigureAwait(false);
            return new ServerResponse<PagedData<PvpBattleDto>> { Data = result };
        }
    }
}
