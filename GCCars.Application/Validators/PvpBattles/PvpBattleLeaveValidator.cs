using GCCars.Application.Data;
using GCCars.Application.Validators.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.PvpBattles
{
    public class PvpBattleLeaveValidator : DbModelValidator<int>
    {
        public PvpBattleLeaveValidator(AppDbContext context) : base(context) { }

        protected override async Task InternalValidate(int model, params object[] data)
        {
            var battle = await db._PvpBattles
                .Include(r => r.Fighters)
                    .ThenInclude(r => r.Car)
                .SingleOrDefaultAsync(r => r.PvpBattleId == model)
                .ConfigureAwait(false);
            if (battle == null)
                AddError("Battle not found.");
            else
            {
                if (battle.StartedAt.HasValue)
                    AddError("Battle has already begun.");
                else
                {
                    if (!battle.Fighters.Any(r => r.Car.OwnerId == (int)data[0]))
                        AddError("You are not fighter of this battle.");
                }
            }
        }
    }
}
