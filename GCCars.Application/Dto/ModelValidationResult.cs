using System.Collections.Generic;

namespace GCCars.Application.Dto
{
    public class ModelValidationResult
    {
        public bool Success => Messages.Count == 0;

        public List<string> Messages { get; set; } = new();
    }
}
