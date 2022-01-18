using System;

namespace GCCars.Application.Dto.Cars
{
    /// <summary>
    /// История продаж машинки.
    /// </summary>
    public class SalesHistoryDto
    {
        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Универсальные дата и время продажи.
        /// </summary>
        public DateTime SoldAt { get; set; }

        /// <summary>
        /// Адрес продавца.
        /// </summary>
        public string Seller { get; set; }

        /// <summary>
        /// Адрес покупателя.
        /// </summary>
        public string Buyer { get; set; }

        /// <summary>
        /// Цена продажи.
        /// </summary>
        public decimal Price { get; set; }
    }
}
