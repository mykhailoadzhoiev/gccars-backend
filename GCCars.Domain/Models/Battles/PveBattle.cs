using GCCars.Domain.Models.Cars;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCCars.Domain.Models.Battles
{
    /// <summary>
    /// PvE сражение.
    /// </summary>
    public class PveBattle
    {
        /// <summary>
        /// Идентификатор PvE сражения.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PveBattleId { get; set; }

        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Номер занятого в результате места.
        /// </summary>
        public byte? Position { get; set; }

        /// <summary>
        /// Полученный в результате опыт.
        /// </summary>
        public int? ExpiriencePoints { get; set; }

        #region FK

        /// <summary>
        /// Машинка.
        /// </summary>
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }

        #endregion
    }
}
