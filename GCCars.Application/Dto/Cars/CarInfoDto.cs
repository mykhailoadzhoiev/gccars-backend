using System.Text.Json.Serialization;

namespace GCCars.Application.Dto.Cars
{
    /// <summary>
    /// Информация о машинке в файле.
    /// </summary>
    public class CarInfoDto
    {
        /// <summary>
        /// Характеристики машинки.
        /// </summary>
        public CarAttribute[] Attributes { get; set; }

        /// <summary>
        /// Описание.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL изображения машинки.
        /// </summary>
        [JsonPropertyName("image")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Название машинки.
        /// </summary>
        public string Name { get; set; }
    }
}
