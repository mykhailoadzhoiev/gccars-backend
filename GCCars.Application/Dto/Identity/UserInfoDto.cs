namespace GCCars.Application.Dto.Identity
{
    /// <summary>
    /// Публичная информация о пользователе.
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Адрес пользователя.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Текущий баланс пользователя.
        /// </summary>
        public decimal Balance { get; set; }
        
        /// <summary>
        /// Количество машинок пользователя
        /// </summary>
        public int CardBalance { get; set; }
    }
}
