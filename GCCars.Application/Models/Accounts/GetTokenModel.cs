namespace GCCars.Application.Models.Accounts
{
    /// <summary>
    /// Модель запроса получения токена пользователя.
    /// </summary>
    public class GetTokenModel
    {
        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string Address { get; set; }
    }
}
