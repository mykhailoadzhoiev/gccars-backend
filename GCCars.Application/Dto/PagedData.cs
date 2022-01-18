namespace GCCars.Application.Dto
{
    /// <summary>
    /// Класс-обёртка для возврата постраничных данных
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedData<T>
    {
        /// <summary>
        /// Текущее число записей с учетом фильтра.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Отобранные данные для текушей страницы.
        /// </summary>
        public T[] Data { get; set; }
    }
}
