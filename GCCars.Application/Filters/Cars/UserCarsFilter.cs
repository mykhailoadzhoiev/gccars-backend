namespace GCCars.Application.Filters.Cars
{
    /// <summary>
    /// Фильтр для получения списка машинок пользователя.
    /// </summary>
    public class UserCarsFilter : BaseFilter
    {
        /// <summary>
        /// Перечень MintId машинок, принадлежащих пользователю, но находящихся вне платформы.
        /// </summary>
        public int[] MintIds { get; set; }
    }
}
