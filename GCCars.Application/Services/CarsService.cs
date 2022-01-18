using AutoMapper;
using AutoMapper.QueryableExtensions;
using GCCars.Application.Data;
using GCCars.Application.Dto;
using GCCars.Application.Dto.Cars;
using GCCars.Application.Dto.PvpBattles;
using GCCars.Application.Extensions;
using GCCars.Application.Filters;
using GCCars.Application.Services.Base;
using GCCars.Application.Settings;
using GCCars.Domain.Enums;
using GCCars.Domain.Models.Cars;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using GCCars.Application.Filters.Cars;

namespace GCCars.Application.Services
{
    /// <summary>
    /// Сервис для работы с машинками в БД.
    /// </summary>
    public class CarsService : BaseDbService
    {
        private readonly IdentityService identityService;
        private readonly AdminUserSettings adminUserSettings;

        public CarsService(
            ILogger<CarsService> logger,
            AppDbContext context,
            IMapper mapper,
            IdentityService identityService,
            IOptions<AdminUserSettings> adminUserSettings) : base(logger, context, mapper)
        {
            this.identityService = identityService;
            this.adminUserSettings = adminUserSettings.Value;
        }

        /// <summary>
        /// Получение списка машинок пользователя для PvE сражения.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="skip">Число записей, которые нужно пропустить.</param>
        /// <param name="take">Чмсло записей, которые нужно получить.</param>
        /// <param name="sorting">Параметры сортировки.</param>
        /// <returns></returns>
        public async Task<PagedData<CarDto>> GetReadyForPvE(int userId, int? skip, int? take, SortingItem[] sorting)
        {
            var query = Db._Cars
                .Where(r => r.OwnerId == userId && r.Parameters.Any(p => p.CarParameter == CarParameter.Health && p.Value == "100") && !r.Fighters.Any(f => f.PvpBattle.FinishedAt == null));

            var totalCount = await query.CountAsync().ConfigureAwait(false);
            var result = await query
                .UseCustomOrder(sorting)
                .UsePagination(skip, take)
                .ProjectTo<CarDto>(Mapper.ConfigurationProvider)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return new PagedData<CarDto> { Data = result, TotalCount = totalCount };
        }

        /// <summary>
        /// Получение списка машинок указанного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список машинок.</returns>
        public async Task<PagedData<UserCarListItemDto>> GetUserCars(int? userId, int[] carIds, int? skip, int? take, SortingItem[] sorting)
        {
            if (carIds == null)
                carIds = Array.Empty<int>();

            var query = Db.Cars
                .Where(r => (r.OwnerId == userId && !r.IsDeleted) || (r.IsDeleted && r.IsMinted && carIds.Contains(r.CarId)));
            var totalCount = await query.CountAsync().ConfigureAwait(false);
            var result = await query
                .UseCustomOrder(sorting)
                .UsePagination(skip, take)
                .ProjectTo<UserCarListItemDto>(Mapper.ConfigurationProvider)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return new PagedData<UserCarListItemDto> { Data = result, TotalCount = totalCount };
        }

        /// <summary>
        /// Получение данных одной машинки.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        public async Task<CarCardDto> GetCard(int carId)
        {
            var data = await Db.Cars
                .Include(r => r.History)
                    .ThenInclude(r => r.Owner)
                .Include(r => r.Parameters)
                .SingleAsync(r => r.CarId == carId)
                .ConfigureAwait(false);
            var result = Mapper.Map<CarCardDto>(data);
            result.SalesHistory = data.History
                .Where(r => r.Event == HistoryEvent.Selled || r.Event == HistoryEvent.Bought)
                .Select(r =>
                {
                    r.EventTime = new DateTime(r.EventTime.Ticks - r.EventTime.Ticks % TimeSpan.TicksPerSecond);
                    return r;
                })
                .GroupBy(r => r.EventTime)
                .Select(r => new SalesHistoryDto
                {
                    SoldAt = r.Key,
                    Buyer = r.FirstOrDefault(d => d.Event == HistoryEvent.Bought)?.Owner.UserName,
                    Seller = r.FirstOrDefault(d => d.Event == HistoryEvent.Selled)?.Owner.UserName,
                    CarId = r.First().CarId,
                    Price = r.First().Price
                })
                .OrderByDescending(x => x.SoldAt)
                .ToArray();
            return result;
        }

        /// <summary>
        /// Получение списка машинок, выставленных на продажу.
        /// </summary>
        /// <returns>Список машинок.</returns>
        public async Task<PagedData<CarDto>> GetOnSale(SearchFilter filter)
        {
            var query = Db.Cars
                .Where(r => !r.IsDeleted && r.IsTradeable);
            var totalCount = await query.CountAsync().ConfigureAwait(false);
            var result = await query
                .GetCarsByName(filter)
                .GetCarsByLevel(filter)
                .GetCarsByAlphabet(filter.Alphabetic)
                .UseCustomOrder(filter.Sorting)
                .UsePagination(filter.Skip, filter.Take)
                .ProjectTo<CarDto>(Mapper.ConfigurationProvider)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return new PagedData<CarDto> { Data = result, TotalCount = totalCount };
        }

        /// <summary>
        /// Проверка существования машинки в БД по имени.
        /// </summary>
        /// <param name="name">Имя машинки.</param>
        /// <returns></returns>
        public async Task<bool> CarExists(string name)
        {
            return await Db._Cars.AnyAsync(r => r.Name == name).ConfigureAwait(false);
        }

        /// <summary>
        /// Создать машинку пользователя в БД из json.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="carInfo">JSON с информацией о машинке.</param>
        /// <returns></returns>
        public async Task<int> CreateCar(int userId, CarInfoDto carInfo, string filePath, bool isTradeable = false)
        {
            var tx = await Db.Database.BeginTransactionAsync().ConfigureAwait(false);

            var car = Mapper.Map<Car>(carInfo);
            car.FileUrl = filePath;
            car.Parameters.Add(new Parameter
            {
                CarParameter = CarParameter.Health,
                Value = "100"
            });
            if (car.Price == 0)
                car.Price = 20;
            car.OwnerId = userId;
            car.IsTradeable = isTradeable;
            await Db.Cars.AddAsync(car).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await AddCarHistory(car.CarId, HistoryEvent.PutInto, car.OwnerId, car.Price).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);

            await tx.CommitAsync().ConfigureAwait(false);

            return car.CarId;
        }

        /// <summary>
        /// Снятие машики с продажи.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        public async Task SaleCancel(int carId)
        {
            var car = await Db.Cars.SingleOrDefaultAsync(r => r.CarId == carId).ConfigureAwait(false);
            car.IsTradeable = false;

            await AddCarHistory(car.CarId, HistoryEvent.WithdrawnFromSale, car.OwnerId, car.Price).ConfigureAwait(false);

            await Db.SaveChangesAsync(false);
        }

        /// <summary>
        /// Выставление машинки на продажу.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <param name="price">Новая цена продажи.</param>
        /// <returns></returns>
        public async Task Sell(int carId, decimal price)
        {
            var car = await Db.Cars.SingleAsync(r => r.CarId == carId);
            car.Price = price;
            car.IsTradeable = true;
            car.LastUpdatedAt = DateTime.UtcNow;

            await AddCarHistory(carId, HistoryEvent.PutupForSale, car.OwnerId, car.Price);

            await Db.SaveChangesAsync();
        }

        /// <summary>
        /// Покупка машинки.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <param name="userId">Идентификатор покупателя.</param>
        /// <returns></returns>
        public async Task Buy(int carId, int userId)
        {
            var car = await Db.Cars.SingleAsync(r => r.CarId == carId).ConfigureAwait(false);
            var prevOwner = await identityService.GetUser(car.OwnerId).ConfigureAwait(false);
            var newOwner = await identityService.GetUser(userId).ConfigureAwait(false);

            if (prevOwner.UserName != adminUserSettings.Address)
                prevOwner.Balance += car.Price;
            newOwner.Balance -= car.Price;

            car.OwnerId = userId;
            car.IsTradeable = false;
            car.LastUpdatedAt = DateTime.UtcNow;

            await AddCarHistory(carId, HistoryEvent.Selled, prevOwner.Id, car.Price).ConfigureAwait(false);
            await AddCarHistory(carId, HistoryEvent.Bought, car.OwnerId, car.Price).ConfigureAwait(false);

            await Db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Получение списка машинок, готовых к сражению.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public async Task<PagedData<CarFighterDto>> GetReadyForPvP(int userId, int? battleId, int? skip, int? take, SortingItem[] sorting)
        {
            // var query = Db._Cars
                // .Where(r => r.OwnerId == userId && r.Parameters.Any(p => p.CarParameter == CarParameter.Health && p.Value == "100") && !r.Fighters.Any(f => f.PvpBattle.FinishedAt == null));
            var query = Db._Cars
                .Where(r => r.OwnerId == userId);
                query = query.Where(r => r.Parameters.Any(p => p.CarParameter == CarParameter.Health && p.Value == "100"));
                query = query.Where(r => !r.Fighters.Any(f => f.PvpBattle.FinishedAt == null));
            if (battleId.HasValue)
            {
                var battle = await Db._PvpBattles.SingleOrDefaultAsync(r => r.PvpBattleId == battleId).ConfigureAwait(false);
                if (battle.Level.HasValue)
                    query = query.Where(r => r.Parameters.Any(p => p.CarParameter == CarParameter.Level && p.Value == battle.Level.ToString()));
            }

            var totalCount = await query.CountAsync().ConfigureAwait(false);
            var result = await query
                .UseCustomOrder(sorting)
                .UsePagination(skip, take)
                .ProjectTo<CarFighterDto>(Mapper.ConfigurationProvider)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return new PagedData<CarFighterDto> { Data = result, TotalCount = totalCount };
        }

        /// <summary>
        /// Вывод машинки из платформы.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        public async Task PushOut(int carId)
        {
            var car = await Db.Cars.SingleAsync(r => r.CarId == carId).ConfigureAwait(false);
            car.IsDeleted = true;
            car.LastUpdatedAt = DateTime.UtcNow;
            car.IsMinted = true;

            await AddCarHistory(carId, HistoryEvent.PushedOut, car.OwnerId, car.Price).ConfigureAwait(false);

            await Db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Добавление записи истории машинки.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <param name="historyEvent">Событие истории (выполняемая операция)</param>
        /// <param name="ownerId">Текущий владелец машинки.</param>
        /// <param name="price">Текущая цена машинки.</param>
        /// <returns></returns>
        private async Task AddCarHistory(int carId, HistoryEvent historyEvent, int ownerId, decimal price)
        {
            var history = new History
            {
                CarId = carId,
                Event = historyEvent,
                OwnerId = ownerId,
                Price = price
            };
            await Db.History.AddAsync(history);
        }

        /// <summary>
        /// Заведение машинки на платформе.
        /// </summary>
        /// <param name="mintId">Идентификатор NFT.</param>
        /// <param name="userId">Идентификатор владельца.</param>
        /// <returns></returns>
        public async Task<int> PutInto(int carId, int userId)
        {
            var car = await Db.Cars.SingleAsync(r => r.CarId == carId).ConfigureAwait(false);
            car.IsDeleted = false;
            car.OwnerId = userId;
            car.LastUpdatedAt = DateTime.UtcNow;

            await AddCarHistory(car.CarId, HistoryEvent.PutInto, car.OwnerId, car.Price).ConfigureAwait(false);

            await Db.SaveChangesAsync().ConfigureAwait(false);

            return car.CarId;
        }
    }
}
