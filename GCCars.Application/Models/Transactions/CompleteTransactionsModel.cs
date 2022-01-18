using System;

namespace GCCars.Application.Models.Transactions
{
    /// <summary>
    /// Модель завершения отслеживания транзакций пользователя.
    /// </summary>
    public class CompleteTransactionsModel
    {
        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Массив с универсальными датой и временем начала каждой транзакции.
        /// </summary>
        public DateTime[] StartTimes { get; set; }
    }
}
