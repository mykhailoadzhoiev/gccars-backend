namespace GCCars.Application.Constants
{
    /// <summary>
    /// Названия методов хаба для связи с клиентом.
    /// </summary>
    public static class HubMethod
    {
        /// <summary>
        /// Уведомление об изменении в списке PvP сражений.
        /// </summary>
        public const string UPDATE_PVP_BATTLES_LIST = "PvpListChanged";

        /// <summary>
        /// Уведомление об изменении данных PvP сражения.
        /// </summary>
        public const string UPDATE_PVP_BATTLE = "PvpBattleChanged";
    }
}
