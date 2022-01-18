namespace GCCars.Application.Filters.Cars
{
    public class SearchFilter : BaseFilter
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public bool Alphabetic { get; set; }
    }
}