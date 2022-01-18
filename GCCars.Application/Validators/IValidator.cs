using System.Collections.Generic;
using System.Threading.Tasks;

namespace GCCars.Application.Validators
{
    /// <summary>
    /// Валидатор модели.
    /// </summary>
    /// <typeparam name="T">Тип валидируемой модели</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Валидация модели.
        /// </summary>
        /// <param name="model">Валидируемая модель.</param>
        /// <param name="data">Дополнительные необязательные параметры</param>
        /// <returns>True, если модель валидна</returns>
        Task<bool> Validate(T model, params object[] data);

        /// <summary>
        /// Чтение списка ошибок при последней валидации.
        /// </summary>
        /// <returns>Список сообщений об ошибках.</returns>
        IEnumerable<string> GetErrors();
    }
}
