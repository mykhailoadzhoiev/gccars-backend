using System;

namespace GCCars.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NestedTableValueAttribute : Attribute
    {
        public string[] ValueNames { get; private set; }

        public NestedTableValueAttribute(string valueNames)
        {
            ValueNames = valueNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
