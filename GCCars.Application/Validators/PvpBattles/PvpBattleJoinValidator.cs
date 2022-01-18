using GCCars.Application.Data;
using GCCars.Application.Extensions;
using GCCars.Application.Validators.Base;
using GCCars.Domain.Enums;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.PvpBattles
{
    public class PvpBattleJoinValidator : DbModelValidator<int>
    {
        private readonly UserManager<AppUser> userManager;

        public PvpBattleJoinValidator(
            AppDbContext context,
            UserManager<AppUser> userManager) : base(context)
        {
            this.userManager = userManager;
        }

        protected override async Task InternalValidate(int model, params object[] data)
        {
            var carId = (int)data[0];
            var car = await db._Cars
                .Include(r => r.Parameters)
                .SingleOrDefaultAsync(r => r.CarId == carId)
                .ConfigureAwait(false);
            if (car == null)
            {
                AddError("Car not found.");
                return;
            }
            if (car.Parameters.First(r => r.CarParameter == CarParameter.Health).Value != "100")
            {
                AddError("Car is damaged.");
                return;
            }
            if (await db.Fighters.AnyAsync(r => r.CarId == carId && r.PvpBattle.FinishedAt == null).ConfigureAwait(false))
            {
                AddError("Car is already in the battle.");
                return;
            }

            var battle = await db._PvpBattles
                .Include(r => r.Fighters)
                    .ThenInclude(r => r.Car)
                .SingleOrDefaultAsync(r => r.PvpBattleId == model)
                .ConfigureAwait(false);
            if (battle == null)
            {
                AddError("Battle not found.");
                return;
            }
            if (battle.Fighters.Any(r => r.Car.OwnerId == car.OwnerId))
            {
                AddError("You are already joined this battle.");
                return;
            }
            if (battle.StartedAt.HasValue)
            {
                AddError("Battle has already begun.");
                return;
            }
            if (battle.MaxFighters == battle.Fighters.Count)
            {
                AddError("Max fighters number is already reached.");
                return;
            }
            if (battle.Level.HasValue && battle.Level != car.Parameters.First(r => r.CarParameter == Domain.Enums.CarParameter.Level).Value.ToInt())
            {
                AddError($"Level {battle.Level} is requred.");
                return;
            }

            var user = await userManager.FindByIdAsync(car.OwnerId.ToString()).ConfigureAwait(false);
            if (user.Balance < battle.BetAmount)
                AddError("You have no enough money to join the battle.");
        }
    }
}
