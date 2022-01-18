using GCCars.Application.Data;

namespace GCCars.Application.Validators.Base
{
    /// <summary>
    /// Базовый класс валидаторов моделей БД.
    /// </summary>
    /// <typeparam name="T">Тип валидируемой модели.</typeparam>
    public abstract class DbModelValidator<T> : BaseValidator<T>
    {
        protected readonly AppDbContext db;

        public DbModelValidator(AppDbContext context)
        {
            db = context;
        }
    }
}
