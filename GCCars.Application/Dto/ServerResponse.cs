using GCCars.Application.Enums;
using System.Collections.Generic;

namespace GCCars.Application.Dto
{
    public class ServerResponse
    {
        public RequestResult Result { get; set; } = RequestResult.Success;

        public IEnumerable<string> Messages { get; set; }

        public static ServerResponse OK => new();
    }

    public class ServerResponse<T>: ServerResponse
    {
        public T Data { get; set; }
    }
}
