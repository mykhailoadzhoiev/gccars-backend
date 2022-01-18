namespace GCCars.Domain.Enums
{
    /// <summary>
    /// События истории машинки.
    /// </summary>
    public enum HistoryEvent : byte
    {
        /// <summary>
        /// Создана на платформе.
        /// </summary>
        Created,

        /// <summary>
        /// Выставлена на продажу.
        /// </summary>
        PutupForSale,

        /// <summary>
        /// Продана другому владельцу.
        /// </summary>
        Selled,

        /// <summary>
        /// Куплена новым владельцем.
        /// </summary>
        Bought,

        /// <summary>
        /// Снята с продажи.
        /// </summary>
        WithdrawnFromSale,

        /// <summary>
        /// Выведена из платформы.
        /// </summary>
        PushedOut,

        /// <summary>
        /// Введена в платформу.
        /// </summary>
        PutInto
    }
}
