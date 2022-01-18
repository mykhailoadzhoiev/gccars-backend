using GCCars.Application.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GCCars.Application.Validators.Base
{
    /// <summary>
    /// Базовый класс валидаторов.
    /// </summary>
    public abstract class BaseValidator<T> : IValidator<T>
    {
        private readonly IList<string> messages = new List<string>();

        /// <summary>
        /// Очистка списка сообщений об ошибках валидации.
        /// </summary>
        protected void ClearErrors()
        {
            messages.Clear();
        }

        /// <summary>
        /// Добавление сообщения об ошибке валидации.
        /// </summary>
        /// <param name="errorMessage"></param>
        protected void AddError(string errorMessage)
        {
            messages.Add(errorMessage);
        }

        /// <summary>
        /// Признак наличия ошибок в проверяемой модели.
        /// </summary>
        /// <returns></returns>
        protected bool HasErrors()
        {
            return messages.Count > 0;
        }

        public async Task<bool> Validate(T model, params object[] data)
        {
            ClearErrors();
            try
            {
                await InternalValidate(model, data);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
            return !HasErrors();
        }

        public virtual IEnumerable<string> GetErrors()
        {
            return messages;
        }

        /// <summary>
        /// Синхронная валидация модели.
        /// </summary>
        /// <typeparam name="T">Тип валидируемой модели.</typeparam>
        /// <param name="model">Валидируемая модель.</param>
        /// <param name="data">Доплнительные параметры.</param>
        /// <returns></returns>
        protected abstract Task InternalValidate(T model, params object[] data);
    }
}
