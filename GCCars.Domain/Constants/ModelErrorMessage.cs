namespace GCCars.Domain.Constants
{
    /// <summary>
    /// Сообщения об ошибках модели на исходном языке.
    /// </summary>
    public static class ModelErrorMessage
    {
        public const string OVERSIZED = "Длина поля \"{0}\" не может быть больше {1}";

        public const string REQUIRED = "Поле \"{0}\" должно быть заполнено.";
    }
}
