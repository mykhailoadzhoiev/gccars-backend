using GCCars.Application.Dto.Cars;
using GCCars.Application.Hubs;
using GCCars.Application.Settings;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace GCCars.Application.Services
{
    /// <summary>
    /// Сервис для выполнения фоновых задач.
    /// </summary>
    public class BackgroundJobsService
    {
        private readonly ILogger<BackgroundJobsService> logger;
        private readonly IpfsService ipfsService;
        private readonly AdminUserSettings cfg;
        private readonly UserManager<AppUser> userManager;
        private readonly CarsService carsService;
        private readonly TransactionsService transactionsService;
        private readonly IHubContext<PlatformHub> platformHubContext;

        public BackgroundJobsService(
            ILogger<BackgroundJobsService> logger,
            IpfsService ipfsService,
            IOptions<AdminUserSettings> options,
            UserManager<AppUser> userManager,
            CarsService carsService,
            TransactionsService transactionsService,
            IHubContext<PlatformHub> platformHubContext)
        {
            this.logger = logger;
            this.ipfsService = ipfsService;
            cfg = options.Value;
            this.userManager = userManager;
            this.carsService = carsService;
            this.transactionsService = transactionsService;
            this.platformHubContext = platformHubContext;
        }

        /// <summary>
        /// Синхронизация машинок администратора.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns></returns>
        public void CarsSynchronization(CancellationToken cancellationToken)
        {
            logger.LogInformation("Run cars synchronization process.");

            try
            {
                var serializerProperties = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                serializerProperties.Converters.Add(new JsonStringEnumConverter());

                var user = userManager.FindByNameAsync(cfg.Address).ConfigureAwait(false).GetAwaiter().GetResult();

                var fileNames = ipfsService.GetFileList(cfg.CarJsonFolder).ConfigureAwait(false).GetAwaiter().GetResult();
                foreach (var fileName in fileNames.Where(r => r.EndsWith(".json")))
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var filePath = $"{cfg.CarJsonFolder}/{fileName}";
                    var carJson = ipfsService.GetFile(filePath).ConfigureAwait(false).GetAwaiter().GetResult();
                    var car = JsonSerializer.Deserialize<CarInfoDto>(carJson, serializerProperties);
                    if (!carsService.CarExists(car.Name).ConfigureAwait(false).GetAwaiter().GetResult())
                        carsService.CreateCar(user.Id, car, $"https://ipfs.io/ipfs/{filePath}", true).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Cars synchronization process has failed due the error: {ex.Message}.");
            }

            logger.LogInformation("Cars synchronization process completed.");
        }

        /// <summary>
        /// Проверка незавершенных транзакций.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void CheckPendingTransactions(CancellationToken cancellationToken)
        {
            var transactions = transactionsService.GetUncompletedTransactions(DateTime.UtcNow.AddMinutes(-1)).ConfigureAwait(false).GetAwaiter().GetResult();
            foreach (var userTransactions in transactions)
            {
                platformHubContext.Clients.Users(userTransactions.UserId.ToString())
                    .SendAsync("CheckTransactions", userTransactions.Address, userTransactions.StartTimes)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
