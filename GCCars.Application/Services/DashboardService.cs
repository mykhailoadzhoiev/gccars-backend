using AutoMapper;
using AutoMapper.QueryableExtensions;
using GCCars.Application.Data;
using GCCars.Application.Dto;
using GCCars.Application.Dto.Cars;
using GCCars.Application.Dto.Dashboard;
using GCCars.Application.Extensions;
using GCCars.Application.Filters;
using GCCars.Application.Services.Base;
using GCCars.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GCCars.Application.Services
{
    public class DashboardService : BaseDbService
    {
        public DashboardService(
            ILogger<DashboardService> logger,
            AppDbContext context,
            IMapper mapper) : base(logger, context, mapper) { }

        /// <summary>
        /// Получить итоговые суммы продаж.
        /// </summary>
        /// <returns></returns>
        public async Task<TotalsDto> GetTotals()
        {
            var result = new TotalsDto
            {
                CurrentCost = await Db._Cars.Where(r => r.IsTradeable).SumAsync(r => r.Price).ConfigureAwait(false),
                TotalSales = await Db._History.Where(r => r.Event == HistoryEvent.Selled).SumAsync(r => r.Price).ConfigureAwait(false)
            };
            return result;
        }

        /// <summary>
        /// Получение списка загруженных в платформу машинок.
        /// </summary>
        /// <param name="skip">Число записей, которые нужно пропустить.</param>
        /// <param name="take">Число записей, которые нужно получить.</param>
        /// <param name="sorting">Параметры сортировки.</param>
        /// <returns></returns>
        public async Task<PagedData<CarListItemDto>> GetPutIntoCars(int? skip, int? take, SortingItem[] sorting)
        {
            var query = Db._History.Where(r => r.Event == HistoryEvent.PutInto);
            var totalCount = await query.CountAsync().ConfigureAwait(false);
            if ((sorting?.Length ?? 0)== 0)
                sorting = new SortingItem[] { new SortingItem { Direction = OrderDirection.Descending, PropertyName = "EventTime" } };
            query = query.UseCustomOrder(sorting);
            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (take.HasValue)
                query = query.Take(take.Value);
            var result = await query
                .Select(r => r.Car)
                .ProjectTo<CarListItemDto>(Mapper.ConfigurationProvider)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return new PagedData<CarListItemDto> { Data = result, TotalCount = totalCount };
        }

        /// <summary>
        /// Получение списка проданных машинок.
        /// </summary>
        /// <param name="skip">Число записей, которые нужно пропустить.</param>
        /// <param name="take">Число записей, которые нужно получить.</param>
        /// <param name="sorting">Параметры сортировки.</param>
        /// <returns></returns>
        public async Task<PagedData<CarListItemDto>> GetRecentlySoldCars(int? skip, int? take, SortingItem[] sorting)
        {
            var query = Db._History.Where(r => r.Event == HistoryEvent.Selled);
            var totalCount = await query.CountAsync().ConfigureAwait(false);
            if ((sorting?.Length ?? 0) == 0)
                sorting = new SortingItem[] { new SortingItem { Direction = OrderDirection.Descending, PropertyName = "EventTime" } };
            var result = await query
                .UseCustomOrder(sorting)
                .UsePagination(skip, take)
                .Select(r => r.Car)
                .ProjectTo<CarListItemDto>(Mapper.ConfigurationProvider)
                .ToArrayAsync()
                .ConfigureAwait(false);
            return new PagedData<CarListItemDto> { Data = result, TotalCount = totalCount };
        }
    }
}
