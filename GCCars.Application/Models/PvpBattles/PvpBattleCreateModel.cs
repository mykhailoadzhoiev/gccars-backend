namespace GCCars.Application.Models.PvpBattles
{
    /// <summary>
    /// Модель создания PvP сражения.
    /// </summary>
    public class PvpBattleCreateModel
    {
        /// <summary>
        /// Уровень машинок.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Максимальное число участников.
        /// </summary>
        public int MaxFighters { get; set; }

        /// <summary>
        /// Свумма ставки.
        /// </summary>
        public decimal BetAmount { get; set; }

        /// <summary>
        /// Идентификатор машинки для боя.
        /// </summary>
        public int CarId { get; set; }
    }
}
