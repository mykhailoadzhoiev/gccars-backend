using GCCars.Application.Data;
using GCCars.Application.Models.Cars;
using GCCars.Application.Validators.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Cars
{
    public class CarSaleValidator : DbModelValidator<CarSaleModel>
    {
        public CarSaleValidator(AppDbContext context) : base(context) { }

        protected override async Task InternalValidate(CarSaleModel model, params object[] data)
        {
            var car = await db.Cars.SingleOrDefaultAsync(r => r.CarId == model.CarId && !r.IsDeleted).ConfigureAwait(false);
            if (car == null)
                AddError("Car not found.");
            else
            {
                if (car.OwnerId != (int)data[0])
                    AddError("You aren't owner of this car.");
                if (car.IsTradeable)
                    AddError("Car is already on sale.");
            }
            if (model.Price <= 0)
                AddError("Price must by positive.");
        }
    }
}
