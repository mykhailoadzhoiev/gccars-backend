using GCCars.Application.Data;
using GCCars.Application.Validators.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Cars
{
    public class CarPushOutValidator : DbModelValidator<int>
    {
        public CarPushOutValidator(AppDbContext context) : base(context) { }

        protected override async Task InternalValidate(int model, params object[] data)
        {
            var car = await db.Cars.SingleOrDefaultAsync(r => r.CarId == model).ConfigureAwait(false);
            if (car == null)
            {
                AddError("Car not found.");
                return;
            }
            if (car.OwnerId != (int)data[0])
            {
                AddError("You're not owner of this car.");
                return;
            }
            if (car.IsDeleted)
                AddError("Car is already pushed out.");
            if (car.IsTradeable)
                AddError("Car is on sale now.");
        }
    }
}
