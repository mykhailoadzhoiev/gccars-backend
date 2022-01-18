using System;

namespace GCCars.Application.Dto.Transactions
{
    /// <summary>
    /// Незавершенные транзакции пользователя.
    /// </summary>
    public class UserTransactionsDto
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Массив универсальных даты и времени начала транзакции.
        /// </summary>
        public DateTime[] StartTimes { get; set; }
    }
}
