using GCCars.Domain.Enums;

namespace GCCars.Application.Filters
{
    /// <summary>
    /// Модель для настройки сортировки в запросе.
    /// </summary>
    public class SortingItem
    {
        /// <summary>
        /// Имя свойства, по которому выполняется сортировка.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Направление сортировки.
        /// </summary>
        public OrderDirection Direction { get; set; }

        /// <summary>
        /// Очередность сортировки (чем выше число, тем менее приоритетна сортировка).
        /// </summary>
        public int SortOrder { get; set; }
    }
}
