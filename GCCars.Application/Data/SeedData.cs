using GCCars.Application.Dto.Cars;
using GCCars.Application.Services;
using GCCars.Domain.Constants;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GCCars.Application.Data
{
    /// <summary>
    /// Первичное наполнение БД.
    /// </summary>
    public static class SeedData
    {
        private static string[] appRoles = { RoleName.SYSTEM_ADMIN, RoleName.CLIENT_USER };
        private const string ADMIN_ADDRESS = "0xa001a001a001a001a001a001a001a001a001a001";

        public static async Task EnsureInitialDataCreated(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            // создаем список пользовательских ролей в приложении
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
            foreach (var role in appRoles)
            {
                if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                {
                    var appRole = new AppRole { Name = role };
                    await roleManager.CreateAsync(appRole).ConfigureAwait(false);
                }
            }

            // создаем аккаунт администратора
            var identityService = scope.ServiceProvider.GetRequiredService<IdentityService>();
            if (!await identityService.UserExists(ADMIN_ADDRESS).ConfigureAwait(false))
                await identityService.RegisterUser(ADMIN_ADDRESS, RoleName.SYSTEM_ADMIN).ConfigureAwait(false);
        }
    }
}
