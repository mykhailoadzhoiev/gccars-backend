using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GCCars.Application.Constants;
using GCCars.Application.Data;
using GCCars.Application.Dto;
using GCCars.Application.Dto.PvpBattles;
using GCCars.Application.Exceptions;
using GCCars.Application.Extensions;
using GCCars.Application.Filters;
using GCCars.Application.Hubs;
using GCCars.Application.Services.Base;
using GCCars.Domain.Models.Battles;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace GCCars.Application.Services
{
    public class PvpBattlesService : BaseDbService
    {
        private readonly IHubContext<PvpBattlesHub> _hubContext;

        public PvpBattlesService(
            ILogger<PvpBattlesService> logger,
            AppDbContext context,
            IMapper mapper,
            IHubContext<PvpBattlesHub> hubContext) : base(logger, context, mapper)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Получение списка доступных сражений.
        /// </summary>
        /// <param name="userId">Идентификатор текущего пользователя.</param>
        /// <param name="skip">Число записей, которые нужно пропустить.</param>
        /// <param name="take">Число записей, которые нужно получить.</param>
        /// <param name="sorting">Параметры сортировки.</param>
        /// <returns></returns>
        public async Task<PagedData<PvpBattleDto>> GetUnstartedBattles(int userId, int? skip, int? take,
            SortingItem[] sorting)
        {
            var query = Db._PvpBattles
                .Where(r => r.StartedAt == null).Include(b => b.Fighters);
            var totalCount = await query.CountAsync().ConfigureAwait(false);
            var result = await query
                .UseCustomOrder(sorting)
                .UsePagination(skip, take)
                .ProjectTo<PvpBattleDto>(Mapper.ConfigurationProvider, new {userId})
                .ToArrayAsync()
                .ConfigureAwait(false);

            await AddAddresses(query, result);

            return new PagedData<PvpBattleDto>
            {
                Data = result,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Получение данных сражения по его идентификатору.
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <returns></returns>
        public async Task<PvpBattleDto> GetBattle(int battleId)
        {
            var result = await Db._PvpBattles
                .Where(r => r.PvpBattleId == battleId)
                .ProjectTo<PvpBattleDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
            if (result == null)
                throw new PublicException($"PvP battle ID = {battleId} not found.");
            return result;
        }

        /// <summary>
        /// Вступление в сражение.
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        public async Task JoinBattle(int userId, int battleId, int carId)
        {
            // добавляем участника
            var fighter = new Fighter
            {
                PvpBattleId = battleId,
                CarId = carId,
            };
            await Db.Fighters.AddAsync(fighter).ConfigureAwait(false);

            // увеличиваем количество игроков
            var battle = await Db.PvpBattles.FirstOrDefaultAsync(b => b.PvpBattleId == battleId);
            battle.FightersCount++;

            await Db.SaveChangesAsync().ConfigureAwait(false);

            // отправляем уведомление клиенту об измениях в сражении
            await _hubContext.Clients.All.SendAsync(HubMethod.UPDATE_PVP_BATTLE, battleId).ConfigureAwait(false);
        }

        /// <summary>
        /// Создание сражения.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя организатора сражения.</param>
        /// <param name="betAmount">Сумма ставки.</param>
        /// <param name="carId">Идентификатор выставляемой на сражение машинки.</param>
        /// <param name="level">Требуемый уровень машинки или null.</param>
        /// <param name="maxFighters">Максимальное число участников.</param>
        /// <returns>Идентификатор сражения.</returns>
        public async Task<int> CreateBattle(int userId, int carId, int? level, int maxFighters, decimal betAmount)
        {
            // создаем сражение
            var battle = new PvpBattle
            {
                BetAmount = betAmount,
                Level = level,
                MaxFighters = maxFighters,
                OwnerId = userId,
                FightersCount = 1
            };
            await Db.PvpBattles.AddAsync(battle).ConfigureAwait(false);
            battle.Fighters.Add(new Fighter
            {
                CarId = carId
            });
            await Db.SaveChangesAsync().ConfigureAwait(false);

            // отправляем уведомление клиенту о необходимости обновить список сражений
            await _hubContext.Clients.All.SendAsync(HubMethod.UPDATE_PVP_BATTLES_LIST).ConfigureAwait(false);

            return battle.PvpBattleId;
        }

        /// <summary>
        /// Выход из сражения.
        /// </summary>
        /// <param name="battleId">Идентификатор сражения.</param>
        /// <param name="userId">Идентификатор владельца машинки.</param>
        /// <returns></returns>
        public async Task LeaveBattle(int battleId, int userId)
        {
            // удаляем участника
            await Db.Fighters
                .Where(r => r.PvpBattleId == battleId && r.Car.OwnerId == userId)
                .DeleteFromQueryAsync()
                .ConfigureAwait(false);

            var battle = await Db.PvpBattles
                .Include(r => r.Fighters)
                .ThenInclude(r => r.Car)
                .SingleOrDefaultAsync(r => r.PvpBattleId == battleId)
                .ConfigureAwait(false);

            battle.FightersCount--;

            if (battle.FightersCount == 0)
            {
                // вышел последний участник, удаляем сражение
                await Db.PvpBattles
                    .Where(r => r.PvpBattleId == battleId)
                    .DeleteFromQueryAsync()
                    .ConfigureAwait(false);
                // отправляем уведомление клиенту об измении списка сражений
                await _hubContext.Clients.All.SendAsync(HubMethod.UPDATE_PVP_BATTLES_LIST).ConfigureAwait(false);
                return;
            }

            // если вышел владелец комнаты, заменяем его
            if (battle.OwnerId == userId)
                battle.OwnerId = battle.Fighters[0].Car.OwnerId;

            await Db.SaveChangesAsync().ConfigureAwait(false);

            // отправляем уведомление клиенту об измениях в сражении
            await _hubContext.Clients.All.SendAsync(HubMethod.UPDATE_PVP_BATTLE, battleId).ConfigureAwait(false);
        }

        private async Task AddAddresses(IQueryable<PvpBattle> query, PvpBattleDto[] result)
        {
            foreach (var battle in await query.ToListAsync())
            {
                foreach (var fighter in battle.Fighters)
                {
                    var carId = (await Db._Cars.FirstOrDefaultAsync(c => c.CarId == fighter.CarId)).OwnerId;
                    var address = (await Db.Users.FirstOrDefaultAsync(u =>
                        u.Id == carId)).UserName;
                    var battleDto = result.FirstOrDefault(r => r.PvpBattleId == battle.PvpBattleId);
                    battleDto.FightersAddresses ??= new List<string>();
                    battleDto.FightersAddresses.Add(address);
                }
            }
        }
    }
}