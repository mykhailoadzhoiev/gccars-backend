using GCCars.Domain.Enums;
using GCCars.Domain.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCCars.Domain.Models.Cars
{
    /// <summary>
    /// Запись истории машинки.
    /// </summary>
    public class History
    {
        /// <summary>
        /// Идентификатор записи.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }

        /// <summary>
        /// Событие истории машинки (операция, выполняемая с машинкой).
        /// </summary>
        public HistoryEvent Event { get; set; }

        /// <summary>
        /// Универсальные дата и время события.
        /// </summary>
        public DateTime EventTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Идентификатор владельца машинки, совершающего операцию.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Цена машинки на момент проведения операции.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        #region FK

        /// <summary>
        /// Машинка.
        /// </summary>
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }

        /// <summary>
        /// Владелец машинки, совершающий операцию.
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public AppUser Owner { get; set; }

        #endregion
    }
}
