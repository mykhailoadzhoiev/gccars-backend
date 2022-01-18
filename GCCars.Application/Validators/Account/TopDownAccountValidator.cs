using GCCars.Application.Validators.Base;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Account
{
    public class TopDownAccountValidator : BaseValidator<decimal>
    {
        private readonly UserManager<AppUser> userManager;

        public TopDownAccountValidator(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        protected override async Task InternalValidate(decimal model, params object[] data)
        {
            if (model <= 0)
            {
                AddError("Withdraw amount must be positive.");
                return;
            }

            var user = await userManager.FindByIdAsync(data[0].ToString()).ConfigureAwait(false);
            if (user.Balance < model)
                AddError("Not enough money to withdraw.");
        }
    }
}
