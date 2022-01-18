using GCCars.Application.Data;
using GCCars.Application.Validators.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Cars
{
    public class CarSaleCancelValidator : DbModelValidator<int>
    {
        public CarSaleCancelValidator(AppDbContext context) : base(context) { }

        protected override async Task InternalValidate(int model, params object[] data)
        {
            var car = await db.Cars.SingleOrDefaultAsync(r => r.CarId == model).ConfigureAwait(false);
            if (car == null)
            {
                AddError("Car not found.");
            }
            else
            {
                if (car.OwnerId != (int)data[0])
                {
                    AddError("You aren't owner of this car.");
                    return;
                }
                if (!car.IsTradeable)
                {
                    AddError("The car is not for sale.");
                }
            }
        }
    }
}
