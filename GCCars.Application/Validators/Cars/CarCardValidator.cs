using GCCars.Application.Data;
using GCCars.Application.Validators.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Cars
{
    public class CarCardValidator : DbModelValidator<int>
    {
        public CarCardValidator(AppDbContext context) : base(context) { }

        protected override async Task InternalValidate(int model, params object[] data)
        {
            var car = await db.Cars.SingleOrDefaultAsync(r => r.CarId == model).ConfigureAwait(false);
            if (car == null)
            {
                AddError("Car not found.");
            }
        }
    }
}
