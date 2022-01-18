using GCCars.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCCars.Domain.Models.Battles
{
    /// <summary>
    /// PvP сражение.
    /// </summary>
    public class PvpBattle
    {
        /// <summary>
        /// Идентификатор сражения.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PvpBattleId { get; set; }

        /// <summary>
        /// Идентификатор пользователя организатора сражения.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Необходимый уровень машинок.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Макисмальное число участников.
        /// </summary>
        public int MaxFighters { get; set; }

        /// <summary>
        /// Ставка.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal BetAmount { get; set; }

        /// <summary>
        /// Универсальные дата и время начала сражения.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Универсальные дата и время окончания сражения.
        /// </summary>
        public DateTime? FinishedAt { get; set; }

        /// <summary>
        /// Количество игроков участвующих в сражении.
        /// </summary>
        public int FightersCount { get; set; }

        #region FK

        /// <summary>
        /// Пользователь организатор сражения.
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public AppUser Owner { get; set; }

        /// <summary>
        /// Участники сражения.
        /// </summary>
        public IList<Fighter> Fighters { get; set; } = new List<Fighter>();

        #endregion
    }
}
