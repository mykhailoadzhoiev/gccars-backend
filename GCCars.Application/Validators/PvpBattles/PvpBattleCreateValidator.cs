using GCCars.Application.Data;
using GCCars.Application.Extensions;
using GCCars.Application.Models.PvpBattles;
using GCCars.Application.Validators.Base;
using GCCars.Domain.Enums;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.PvpBattles
{
    public class PvpBattleCreateValidator : DbModelValidator<PvpBattleCreateModel>
    {
        private readonly UserManager<AppUser> userManager;

        public PvpBattleCreateValidator(
            AppDbContext context,
            UserManager<AppUser> userManager) : base(context)
        {
            this.userManager = userManager;
        }

        protected override async Task InternalValidate(PvpBattleCreateModel model, params object[] data)
        {
            if (model.BetAmount <= 0)
                AddError("Bet amount must be positive.");
            if (model.MaxFighters > 10 || model.MaxFighters == 0)
                AddError("Valid fighters number must be between 1 and 10.");
            if (model.Level.HasValue && (model.Level < 0 || model.Level > 5))
                AddError("If you specify level, it must be between 1 and 5.");
            if (HasErrors())
                return;

            var user = await userManager.FindByIdAsync(((int)data[0]).ToString()).ConfigureAwait(false);
            if (user.Balance < model.BetAmount)
            {
                AddError("Not enough money on balance.");
                return;
            }

            var car = await db._Cars
                .Include(r => r.Parameters)
                .SingleOrDefaultAsync(r => r.CarId == model.CarId)
                .ConfigureAwait(false);
            if (car == null)
            {
                AddError("Car not found.");
                return;
            }
            if (car.OwnerId != (int)data[0])
                AddError("You are not owner of this car.");
            else
            {
                if (model.Level.HasValue && car.Parameters.First(r => r.CarParameter == CarParameter.Level).Value.ToInt() != model.Level)
                    AddError("Car has an inappropriate level.");
                if (car.Parameters.First(r => r.CarParameter == CarParameter.Health).Value != "100")
                    AddError("Car is damaged.");
                if (await db._Fighters.AnyAsync(r => r.CarId == model.CarId && r.PvpBattle.FinishedAt == null))
                    AddError("Car is already in battle.");
            }
        }
    }
}
