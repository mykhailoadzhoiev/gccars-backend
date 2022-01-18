using System;

namespace GCCars.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PropertySynonymsAttribute : Attribute
    {
        public string[] Synonyms { get; private set; } = Array.Empty<string>();

        public PropertySynonymsAttribute(string synonyms)
        {
            Synonyms = synonyms.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
