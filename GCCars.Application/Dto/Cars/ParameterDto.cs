using GCCars.Domain.Enums;

namespace GCCars.Application.Dto.Cars
{
    /// <summary>
    /// Характеристика машинки для отображения на фронте.
    /// </summary>
    public class ParameterDto
    {
        /// <summary>
        /// Идентификатор записи.
        /// </summary>
        public int ParameterId { get; set; }

        /// <summary>
        /// Тип зарактеристики.
        /// </summary>
        public CarParameter CarParameter { get; set; }

        /// <summary>
        /// Значение характеристики.
        /// </summary>
        public string Value { get; set; }
    }
}
