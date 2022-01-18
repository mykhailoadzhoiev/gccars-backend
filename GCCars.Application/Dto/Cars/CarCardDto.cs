namespace GCCars.Application.Dto.Cars
{
    /// <summary>
    /// Данные для карточки машинки.
    /// </summary>
    public class CarCardDto : CarDto
    {
        /// <summary>
        /// Данные по истории продаж.
        /// </summary>
        public SalesHistoryDto[] SalesHistory { get; set; }
    }
}
