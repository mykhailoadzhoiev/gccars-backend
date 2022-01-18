using GCCars.Application.Mapping;
using GCCars.Application.Services;
using GCCars.Application.Validators.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace GCCars.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<BackgroundJobsService>();
            services.AddScoped<IdentityService>();
            services.AddScoped<CarsService>();
            services.AddScoped<IpfsService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<PvpBattlesService>();
            services.AddScoped<PveBattlesService>();
            services.AddScoped<TransactionsService>();

            return services;
        }

        public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(new[]
            {
                typeof(CarsMappingProfile),
                typeof(IdentityMappingProfile),
                typeof(PvpBattlesMappingProfile),
                typeof(PveBattlesMappingProfile)
            });

            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            var validatorBaseType = typeof(BaseValidator<>);
            var types = Assembly.GetAssembly(validatorBaseType)
                .GetTypes();
            var validatorTypes = types
                .Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && !x.IsNested && x.Namespace.StartsWith("GCCars.Application.Validators"))
                .ToArray();
            foreach (var validatorType in validatorTypes)
            {
                services.AddTransient(validatorType);
            }

            return services;
        }
    }
}
