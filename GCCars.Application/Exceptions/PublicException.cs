using System;
using System.Runtime.Serialization;

namespace GCCars.Application.Exceptions
{
    /// <summary>
    /// Класс исключения, обрабатываемого приложением.
    /// Сообщения от этого исключения выводятся клиенту.
    /// </summary>
    public class PublicException : Exception
    {
        public PublicException() { }

        public PublicException(string message) : base(message) { }

        public PublicException(string message, Exception innerException) : base(message, innerException) { }

        public PublicException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
    }
}
