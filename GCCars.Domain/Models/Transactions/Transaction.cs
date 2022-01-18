using GCCars.Domain.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCCars.Domain.Models.Transactions
{
    /// <summary>
    /// Информация о незавершенной транзакции.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Идентификатор записи о транзакции.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        /// <summary>
        /// Адрес пользователя, инициировавшего транзакцию.
        /// </summary>
        [StringLength(42, ErrorMessage = ModelErrorMessage.OVERSIZED)]
        public string Address { get; set; }

        /// <summary>
        /// Универсальные дата и время запроса на транзакцию.
        /// </summary>
        public DateTime StartTime { get; set; }
    }
}
