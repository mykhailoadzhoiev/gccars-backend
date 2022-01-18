using AutoMapper;
using AutoMapper.QueryableExtensions;
using GCCars.Application.Data;
using GCCars.Application.Dto.PveBattles;
using GCCars.Application.Services.Base;
using GCCars.Domain.Models.Battles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GCCars.Application.Services
{
    public class PveBattlesService : BaseDbService
    {
        public PveBattlesService(
            ILogger<PveBattlesService> logger,
            AppDbContext context,
            IMapper mapper) : base(logger, context, mapper)
        {
        }

        public async Task<PveBattleResultDto> StartAndGetResult(int carId)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var battle = new PveBattle
            {
                CarId = carId,
                ExpiriencePoints = 110,
                Position = (byte)rnd.Next(1, 4)
            };
            await Db.PveBattles.AddAsync(battle);
            await Db.SaveChangesAsync().ConfigureAwait(false);
            return new PveBattleResultDto
            {
                ExpiriencePoints = battle.ExpiriencePoints.Value,
                Position = battle.Position.Value,
                PveBattleId = battle.PveBattleId
            };
        }

        public async Task CompleteBattle(int battleId)
        {
            await Db.PveBattles
                .Where(r => r.PveBattleId == battleId)
                .DeleteFromQueryAsync()
                .ConfigureAwait(false);
        }

        public async Task<PveBattleResultDto[]> GetUncompleted(int userId)
        {
            var result = await Db._PveBattles
                .Where(r => r.Car.OwnerId == userId)
                .ProjectTo<PveBattleResultDto>(Mapper.ConfigurationProvider)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return result;
        }
    }
}
