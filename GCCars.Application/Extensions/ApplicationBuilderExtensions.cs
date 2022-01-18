using GCCars.Application.Middleware;
using Microsoft.AspNetCore.Builder;

namespace GCCars.Application.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ErrorHandlingMiddleware>();

            return builder;
        }
    }
}
