using GCCars.Domain.Models.Cars;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GCCars.Domain.Models.Identity;

namespace GCCars.Domain.Models.Battles
{
    /// <summary>
    /// Машинка участник сражения.
    /// </summary>
    public class Fighter
    {
        /// <summary>
        /// Идентификатор участника сражения.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FighterId { get; set; }

        /// <summary>
        /// Идентификатор сражения.
        /// </summary>
        public int PvpBattleId { get; set; }

        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        #region FK

        /// <summary>
        /// Сражение.
        /// </summary>
        [ForeignKey(nameof(PvpBattleId))]
        public PvpBattle PvpBattle { get; set; }

        /// <summary>
        /// Машинка.
        /// </summary>
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }

        #endregion
    }
}