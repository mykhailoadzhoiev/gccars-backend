namespace GCCars.Application.Dto.Dashboard
{
    /// <summary>
    /// Элемент списка машинок на дашборде.
    /// </summary>
    public class CarListItemDto
    {
        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Ссылка на изображение.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Цвет.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Название.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес владельца.
        /// </summary>
        public string OwnerAddress { get; set; }

        /// <summary>
        /// Цена.
        /// </summary>
        public decimal Price { get; set; }
    }
}
