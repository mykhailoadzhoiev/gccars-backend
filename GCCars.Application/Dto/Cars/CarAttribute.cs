using GCCars.Domain.Enums;
using System.Text.Json.Serialization;

namespace GCCars.Application.Dto.Cars
{
    /// <summary>
    /// Характеристика машинки.
    /// </summary>
    public class CarAttribute
    {
        /// <summary>
        /// Тип характеристики.
        /// </summary>
        [JsonPropertyName("trait_type")]
        public CarParameter Type { get; set; }

        /// <summary>
        /// Значение.
        /// </summary>
        public string Value { get; set; }
    }
}
