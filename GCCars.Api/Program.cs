using GCCars.Application.Data;
using GCCars.Application.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GCCars.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            SeedData.EnsureInitialDataCreated(host.Services).GetAwaiter().GetResult();

            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;

            using var scope = host.Services.CreateScope();
            var backgroundJobs = scope.ServiceProvider.GetRequiredService<BackgroundJobsService>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //backgroundJobs.CarsSynchronization(cancellationToken);
            JobStorage.Current = new SqlServerStorage(context.Database.GetConnectionString());
            //RecurringJob.AddOrUpdate("CarsSynchronization", () => backgroundJobs.CarsSynchronization(cancellationToken), Cron.Hourly);
            RecurringJob.AddOrUpdate("CheckTransactions", () => backgroundJobs.CheckPendingTransactions(cancellationToken), Cron.Minutely);

            host.Run();

            tokenSource.Cancel();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
