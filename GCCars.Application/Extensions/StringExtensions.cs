using System.Globalization;

namespace GCCars.Application.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Преобразование строки в целое число.
        /// </summary>
        /// <param name="value">Строковое представление целого числа.</param>
        /// <returns>Целое число.</returns>
        public static int? ToInt(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }
    }
}
