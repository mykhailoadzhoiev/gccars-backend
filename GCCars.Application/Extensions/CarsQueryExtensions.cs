using System.Linq;
using GCCars.Application.Filters.Cars;
using GCCars.Domain.Enums;
using GCCars.Domain.Models.Cars;

namespace GCCars.Application.Extensions
{
    public static class CarsQueryExtensions
    {
        public static IQueryable<Car> GetCarsByName(this IQueryable<Car> query, SearchFilter filter)
        {
            if (filter.Name is null) return query;
            query = query.Where(c => c.Name.ToLower().Contains(filter.Name.ToLower()) );
            return query;
        }

        public static IQueryable<Car> GetCarsByLevel(this IQueryable<Car> query, SearchFilter filter)
        {
            if (filter.Level is null) return query;
            query = query.Where(c =>
                c.Parameters.FirstOrDefault(p => p.CarParameter == CarParameter.Level).Value == filter.Level);
            return query;
        }

        public static IQueryable<Car> GetCarsByAlphabet(this IQueryable<Car> query, bool? isFilter)
        {
            if (isFilter is null || !isFilter.Value) return query;
            query = query.OrderBy(c => c.Name);
            return query;
        }
    }
}