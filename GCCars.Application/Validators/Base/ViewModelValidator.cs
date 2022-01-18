using GCCars.Application.Data;

namespace GCCars.Application.Validators.Base
{
    /// <summary>
    /// Базовый класс валидаторов моделей представления с использованием базы данных.
    /// </summary>
    /// <typeparam name="T">Тип валидируемой модели.</typeparam>
    public abstract class ViewModelValidator<T> : DbModelValidator<T> where T : class
    {
        protected ViewModelValidator(AppDbContext context) : base(context)
        {
        }
    }
}
