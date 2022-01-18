namespace GCCars.Application.Models.Accounts
{
    /// <summary>
    /// Модель запросов изменения баланса пользователя.
    /// </summary>
    public class BalanceChangeModel
    {
        /// <summary>
        /// Величина изменения баланса.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
