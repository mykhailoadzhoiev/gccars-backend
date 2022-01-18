using System;

namespace GCCars.Application.Models.Transactions
{
    /// <summary>
    /// Модель с данными о начале транзакции.
    /// </summary>
    public class StartTransactionModel
    {
        /// <summary>
        /// Адрес пользователя, начавшего транзакцию.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Универсальное время запроса на начало транзакции.
        /// </summary>
        public DateTime StartTime { get; set; }
    }
}
