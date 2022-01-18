using GCCars.Application.Validators.Base;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Account
{
    public class UserAddressValidator : BaseValidator<string>
    {
        protected override async Task InternalValidate(string model, params object[] data)
        {
            if (string.IsNullOrWhiteSpace(model))
                AddError("User address must have a value.");
            if (model != null && (model.Length != 42 || !model.StartsWith("0x")))
                AddError("User address is not in the correct format.");
            await Task.CompletedTask;
        }
    }
}
