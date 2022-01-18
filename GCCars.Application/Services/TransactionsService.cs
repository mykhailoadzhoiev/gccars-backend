using AutoMapper;
using AutoMapper.QueryableExtensions;
using GCCars.Application.Data;
using GCCars.Application.Dto.Transactions;
using GCCars.Application.Services.Base;
using GCCars.Domain.Models.Identity;
using GCCars.Domain.Models.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GCCars.Application.Services
{
    public class TransactionsService : BaseDbService
    {
        private readonly UserManager<AppUser> userManager;

        public TransactionsService(
            ILogger<TransactionsService> logger,
            AppDbContext context,
            IMapper mapper,
            UserManager<AppUser> userManager) : base(logger, context, mapper)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Постановка транзакции в очередь на отслеживание.
        /// </summary>
        /// <param name="address">Адрес пользователя.</param>
        /// <param name="startTime">Время создания транзакции.</param>
        /// <returns></returns>
        public async Task StartTransactionTracking(string address, DateTime startTime)
        {
            var transaction = new Transaction
            {
                Address = address,
                StartTime = startTime
            };
            await Db.Transactions.AddAsync(transaction).ConfigureAwait(false);
            await Db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Удаление транзакций из очереди на отслеживание.
        /// </summary>
        /// <param name="address">Адрес пользователя.</param>   
        /// <param name="startTimes">Массив с универсальными датой и временем начала транзакций.</param>
        /// <returns></returns>
        public async Task CompleteTransactionsTracking(string address, DateTime[] startTimes)
        {
            await Db.Transactions
                .Where(r => r.Address == address && startTimes.Contains(r.StartTime))
                .DeleteFromQueryAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Получение незавершенных транзакций пользователей.
        /// </summary>
        /// <returns></returns>
        public async Task<UserTransactionsDto[]> GetUncompletedTransactions(DateTime startTime)
        {
            var result = (await Db._Transactions
                .Where(r => r.StartTime <= startTime)
                .Join(userManager.Users.AsNoTracking(), r => r.Address, r => r.UserName, (t, u) => new
                {
                    Address = t.Address,
                    UserId = u.Id,
                    StartTime = t.StartTime
                })
                .ToArrayAsync()
                .ConfigureAwait(false))
                .GroupBy(r => new { r.Address, r.UserId })
                .Select(r => new UserTransactionsDto
                {
                    Address = r.Key.Address,
                    UserId = r.Key.UserId,
                    StartTimes = r.Select(d => d.StartTime).ToArray()
                })
                .ToArray();
            return result;
        }
    }
}
