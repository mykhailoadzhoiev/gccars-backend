namespace GCCars.Application.Filters
{
    /// <summary>
    /// Базовый класс для фиьтрации и сортировкт в запросе.
    /// </summary>
    public class BaseFilter
    {
        /// <summary>
        /// Настройки сортировки.
        /// </summary>
        public SortingItem[] Sorting { get; set; }

        /// <summary>
        /// Число записей, которые нужно пропустить.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Число записей, которые нужно вернуть.
        /// </summary>
        public int? Take { get; set; }
    }
}
