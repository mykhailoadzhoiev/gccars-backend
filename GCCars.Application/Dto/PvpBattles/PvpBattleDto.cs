using System.Collections.Generic;

namespace GCCars.Application.Dto.PvpBattles
{
    /// <summary>
    /// Pvp сражение.
    /// </summary>
    public class PvpBattleDto
    {
        /// <summary>
        /// Идентификатор сражения
        /// </summary>
        public int PvpBattleId { get; set; }

        /// <summary>
        /// Имя пользователя организатора сражения.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// Адрес пользователя организатора сражения.
        /// </summary>
        public string OwnerAddress { get; set; }

        /// <summary>
        /// Уровень машинок.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Текущее число участников.
        /// </summary>
        public int FightersCount { get; set; }

        /// <summary>
        /// Максимальное число участников.
        /// </summary>
        public int MaxFighters { get; set; }

        /// <summary>
        /// Сумма ставки.
        /// </summary>
        public decimal BetAmount { get; set; }

        /// <summary>
        /// Признак того, что текущий пользователь участвует в сражении.
        /// </summary>
        public bool IsParticipate { get; set; }
        
        /// <summary>
        /// Массив пользователей, которые учавствуют в бою.
        /// </summary>
        public List<string> FightersAddresses { get; set; }
    }
}
