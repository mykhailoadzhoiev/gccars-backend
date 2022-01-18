using GCCars.Application.Filters;
using GCCars.Domain.Attributes;
using GCCars.Domain.Enums;
using GCCars.Domain.Models.Cars;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GCCars.Application.Filters.Cars;

namespace GCCars.Application.Extensions
{
    public static class LinqExtensions
    {
        public static Expression<Func<T, object>> GetPropertySelector<T>(this IQueryable<T> query, string propertyName)
        {
            var properties = typeof(T).GetProperties();
            var propertyInfo = properties.FirstOrDefault(r =>
                r.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyInfo == null)
            {
                var propertiesInfo = properties
                    .Where(r => r.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(PropertySynonymsAttribute))))
                    .ToArray();
                foreach (var pi in propertiesInfo)
                {
                    var attribute = pi.GetCustomAttribute<PropertySynonymsAttribute>();
                    if (attribute.Synonyms.Any(r =>
                            r.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        propertyInfo = pi;
                        break;
                    }
                }

                if (propertyInfo == null)
                    return null;
            }

            var arg = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(arg, propertyInfo.Name);
            var convertedProperty = Expression.Convert(property, typeof(object));
            var result = Expression.Lambda<Func<T, object>>(convertedProperty, new ParameterExpression[] {arg});
            return result;
        }

        public static IQueryable<T> UseCustomOrder<T>(this IQueryable<T> query, SortingItem[] sorting)
        {
            if (sorting == null)
                return query;

            var ordered = false;
            foreach (var sort in sorting.Where(r => r.Direction != OrderDirection.None).OrderBy(r => r.SortOrder))
            {
                var propertySelector = query.GetPropertySelector(sort.PropertyName);
                if (propertySelector == null)
                {
                    query = query.UseNestedTableOrder(sort, ordered);
                    ordered = true;
                }
                else
                {
                    if (ordered)
                    {
                        query = sort.Direction == OrderDirection.Ascending
                            ? ((IOrderedQueryable<T>) query).ThenBy(propertySelector)
                            : ((IOrderedQueryable<T>) query).ThenByDescending(propertySelector);
                    }
                    else
                    {
                        ordered = true;
                        query = sort.Direction == OrderDirection.Ascending
                            ? query.OrderBy(propertySelector)
                            : query.OrderByDescending(propertySelector);
                    }
                }
            }

            return query;
        }

        public static IQueryable<T> UsePagination<T>(this IQueryable<T> query, int? skip, int? take)
        {
            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (take.HasValue)
                query = query.Take(take.Value);
            return query;
        }

        private static IQueryable<T> UseNestedTableOrder<T>(this IQueryable<T> query, SortingItem sortingItem,
            bool ordered)
        {
            var propertiesInfo = typeof(T).GetProperties()
                .Where(r => r.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(NestedTableValueAttribute))))
                .ToArray();
            if (typeof(T).Equals(typeof(Car)))
            {
                foreach (var propertyInfo in propertiesInfo)
                {
                    var attribute = propertyInfo.GetCustomAttribute<NestedTableValueAttribute>();
                    if (attribute.ValueNames.Any(r =>
                            r.Equals(sortingItem.PropertyName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (propertyInfo.Name == nameof(Car.Parameters))
                        {
                            var carParameter =
                                (CarParameter) Enum.Parse(typeof(CarParameter), sortingItem.PropertyName, true);
                            if (ordered)
                            {
                                if (sortingItem.Direction == OrderDirection.Ascending)
                                    query = (IQueryable<T>) ((IOrderedQueryable<Car>) query).ThenBy(r =>
                                        Convert.ToInt32(r.Parameters.First(p => p.CarParameter == carParameter).Value));
                                else
                                    query = (IQueryable<T>) ((IOrderedQueryable<Car>) query).ThenByDescending(r =>
                                        Convert.ToInt32(r.Parameters.First(p => p.CarParameter == carParameter).Value));
                            }
                            else
                            {
                                if (sortingItem.Direction == OrderDirection.Ascending)
                                    query = (IQueryable<T>) ((IQueryable<Car>) query).OrderBy(r =>
                                        Convert.ToInt32(r.Parameters.First(p => p.CarParameter == carParameter).Value));
                                else
                                    query = (IQueryable<T>) ((IQueryable<Car>) query).OrderByDescending(r =>
                                        Convert.ToInt32(r.Parameters.First(p => p.CarParameter == carParameter).Value));
                            }
                        }

                        break;
                    }
                }
            }

            return query;
        }
    }
}