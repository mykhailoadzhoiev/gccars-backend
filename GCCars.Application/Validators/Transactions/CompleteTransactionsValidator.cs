using GCCars.Application.Models.Transactions;
using GCCars.Application.Validators.Base;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Transactions
{
    public class CompleteTransactionsValidator : BaseValidator<CompleteTransactionsModel>
    {
        private readonly UserManager<AppUser> userManager;

        public CompleteTransactionsValidator(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        protected override async Task InternalValidate(CompleteTransactionsModel model, params object[] data)
        {
            var user = await userManager.FindByNameAsync(model.Address).ConfigureAwait(false);
            if (user == null)
            {
                AddError("User with given address not found.");
                return;
            }
            if (user.Id != (int)data[0])
                AddError("You don't have permissions to complete this operation.");
        }
    }
}
