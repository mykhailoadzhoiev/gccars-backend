using GCCars.Application.Validators.Base;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Account
{
    public class TopUpAccountValidator : BaseValidator<decimal>
    {
        protected override async Task InternalValidate(decimal model, params object[] data)
        {
            if (model <= 0)
                AddError("Amount value must be positive.");
            await Task.CompletedTask;
        }
    }
}
