using GCCars.Application.Models.Transactions;
using GCCars.Application.Validators.Base;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Transactions
{
    public class StartTransactionValidator : BaseValidator<StartTransactionModel>
    {
        private readonly UserManager<AppUser> userManager;

        public StartTransactionValidator(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        protected override async Task InternalValidate(StartTransactionModel model, params object[] data)
        {
            var user = await userManager.FindByNameAsync(model.Address).ConfigureAwait(false);
            if (user == null)
            {
                AddError("User with given address not found.");
                return;
            }
            if (user.Id != (int)data[0])
            {
                AddError("You don't have permissions to complete this operation.");
                return;
            }
            if (model.StartTime > DateTime.UtcNow)
                AddError("Wrong transaction time.");
        }
    }
}
