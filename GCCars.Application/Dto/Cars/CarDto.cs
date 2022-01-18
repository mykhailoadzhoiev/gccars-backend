using System;
using System.Collections.Generic;

namespace GCCars.Application.Dto.Cars
{
    /// <summary>
    /// Машинка для отображения на фронте.
    /// </summary>
    public class CarDto
    {
        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Описание машинки.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL изображения машинки.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Название машинки.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес владельца машинки.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Универсальные дата и время последнего значимого изменения данных.
        /// </summary>
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        /// Признак того, что машинка выставлена на продажу
        /// </summary>
        public bool IsTradeable { get; set; }
        
        /// <summary>
        /// Цена машики.
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Признак того, что машинка выводилась из платформы.
        /// </summary>
        public bool IsMinted { get; set; }

        /// <summary>
        /// Перечень текущих характеристик машинки.
        /// </summary>
        public IEnumerable<ParameterDto> Parameters { get; set; }
    }
}
