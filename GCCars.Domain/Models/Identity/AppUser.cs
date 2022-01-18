using GCCars.Domain.Models.Cars;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCCars.Domain.Models.Identity
{
    /// <summary>
    /// Учетная запись пользователя приложения
    /// </summary>
    public class AppUser : IdentityUser<int>
    {
        /// <summary>
        /// Текущий баланс пользователя.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Balance { get; set; } = 0;

        /// <summary>
        /// Список машинок пользователя.
        /// </summary>
        public IList<Car> Cars { get; set; } = new List<Car>();

        /// <summary>
        /// История всех машинок пользователя.
        /// </summary>
        public IList<History> History { get; set; } = new List<History>();
    }
}
