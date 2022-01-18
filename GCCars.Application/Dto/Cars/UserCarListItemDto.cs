namespace GCCars.Application.Dto.Cars
{
    /// <summary>
    /// Машинка для отображения в списке пользователя.
    /// </summary>
    public class UserCarListItemDto : CarDto
    {
        /// <summary>
        /// Ссылка на файл-источник.
        /// </summary>
        public string FileUrl { get; set; }
        
        /// <summary>
        /// Выведена ли машинка с платформы сейчас
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
