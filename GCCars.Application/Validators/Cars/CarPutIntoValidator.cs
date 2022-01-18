using GCCars.Application.Data;
using GCCars.Application.Validators.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Cars
{
    public class CarPutIntoValidator : DbModelValidator<int>
    {
        public CarPutIntoValidator(AppDbContext context) : base(context) { }

        protected override async Task InternalValidate(int model, params object[] data)
        {
            var car = await db.Cars.SingleOrDefaultAsync(r => r.CarId == model).ConfigureAwait(false);
            if (car == null)
            {
                AddError("Mint ID not found.");
                return;
            }
            if (!car.IsMinted)
            {
                AddError("Car was not minted yet.");
                return;
            }
            if (!car.IsDeleted)
            {
                AddError("NFT is already put into.");
                return;
            }
        }
    }
}
