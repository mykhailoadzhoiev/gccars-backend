namespace GCCars.Application.Settings
{
    /// <summary>
    /// Пользовательские настройки администратора.
    /// </summary>
    public class AdminUserSettings
    {
        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Папка для фонового обновления списка машинок.
        /// </summary>
        public string CarJsonFolder { get; set; }
    }
}
