using GCCars.Domain.Constants;
using GCCars.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCCars.Domain.Models.Cars
{
    /// <summary>
    /// Характеристика конкретной машинки.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Идентификатор характеристики машинки.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParameterId { get; set; }

        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Характеристика.
        /// </summary>
        public CarParameter CarParameter { get; set; }

        /// <summary>
        /// Значение характеристики в символьном представлении.
        /// </summary>
        [StringLength(50, ErrorMessage = ModelErrorMessage.OVERSIZED)]
        [Required(ErrorMessage = ModelErrorMessage.REQUIRED)]
        public string Value { get; set; }

        #region FK

        /// <summary>
        /// Машинка.
        /// </summary>
        [ForeignKey(nameof(CarId))]
        public Car Car { get; set; }

        #endregion
    }
}
