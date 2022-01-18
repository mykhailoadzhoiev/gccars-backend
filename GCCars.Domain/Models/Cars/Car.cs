using GCCars.Domain.Attributes;
using GCCars.Domain.Constants;
using GCCars.Domain.Models.Battles;
using GCCars.Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCCars.Domain.Models.Cars
{
    /// <summary>
    /// Машинка.
    /// </summary>
    public class Car
    {
        /// <summary>
        /// Идентификатор машинки.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CarId { get; set; }

        /// <summary>
        /// Идентификатор владельца машинки.
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Описание.
        /// </summary>
        [StringLength(1000, ErrorMessage = ModelErrorMessage.OVERSIZED)]
        public string Description { get; set; }

        /// <summary>
        /// URL изображения машинки.
        /// </summary>
        [StringLength(500, ErrorMessage = ModelErrorMessage.OVERSIZED)]
        [Required(ErrorMessage = ModelErrorMessage.REQUIRED)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// URL файла-источника.
        /// </summary>
        [StringLength(500, ErrorMessage = ModelErrorMessage.OVERSIZED)]
        public string FileUrl { get; set; }

        /// <summary>
        /// Название машинки.
        /// </summary>
        [StringLength(100, ErrorMessage = ModelErrorMessage.OVERSIZED)]
        [Required(ErrorMessage = ModelErrorMessage.REQUIRED)]
        public string Name { get; set; }

        /// <summary>
        /// Признак того, что машинка выставлена на продажу.
        /// </summary>
        public bool IsTradeable { get; set; } = false;

        /// <summary>
        /// Текущая цена продажи.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        /// <summary>
        /// Признак того, что машинка выведена из платформы.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Универсальные дата и время последнего значимого изменения данных.
        /// </summary>
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Признак того, что машинка выводилась с платформы.
        /// </summary>
        public bool IsMinted { get; set; } = false;

        #region FK

        /// <summary>
        /// Владелец машинки.
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public AppUser Owner { get; set; }

        /// <summary>
        /// Характеристики машинки.
        /// </summary>
        [NestedTableValue("Level,Health")]
        public IList<Parameter> Parameters { get; set; } = new List<Parameter>();

        /// <summary>
        /// История машинки.
        /// </summary>
        public IList<History> History { get; set; } = new List<History>();

        /// <summary>
        /// Участие в PvP сражениях.
        /// </summary>
        public IList<Fighter> Fighters { get; set; } = new List<Fighter>();

        /// <summary>
        /// Незавершенные PvE сражения (по ним не было реакции пользователя на модальное окно с результатами боя).
        /// </summary>
        public IList<PveBattle> PveBattles { get; set; } = new List<PveBattle>();

        #endregion
    }
}
