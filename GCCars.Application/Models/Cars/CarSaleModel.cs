namespace GCCars.Application.Models.Cars
{
    /// <summary>
    /// Модель для продажи машинки.
    /// </summary>
    public class CarSaleModel
    {
        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Цена продажи.
        /// </summary>
        public decimal Price { get; set; }
    }
}
