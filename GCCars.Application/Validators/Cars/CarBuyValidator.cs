using GCCars.Application.Data;
using GCCars.Application.Validators.Base;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Cars
{
    public class CarBuyValidator : DbModelValidator<int>
    {
        private readonly UserManager<AppUser> userManager;

        public CarBuyValidator(
            AppDbContext context,
            UserManager<AppUser> userManager) : base(context)
        {
            this.userManager = userManager;
        }

        protected override async Task InternalValidate(int carId, params object[] data)
        {
            var car = await db.Cars.SingleOrDefaultAsync(r => r.CarId == carId && !r.IsDeleted).ConfigureAwait(false);
            if (car == null)
                AddError("Car not found.");
            else
            {
                if (!car.IsTradeable)
                    AddError("This car isn't for sale.");
                if (car.OwnerId == (int)data[0])
                {
                    AddError("You're already owner of this car.");
                    return;
                }
                var user = await userManager.FindByIdAsync(((int)data[0]).ToString()).ConfigureAwait(false);
                if (car.Price > user.Balance)
                    AddError("You have not enough money to purchase this car.");
            }
        }
    }
}
