using GCCars.Application.Dto;
using GCCars.Application.Enums;
using GCCars.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GCCars.Application.Middleware
{
    /// <summary>
    /// Глобальный обработчик ошибок. Возвращает стандартный ответ приложения, если исключение было сгенерировано бизнес-слоем приложения.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (PublicException ex)
            {
                var result = new ServerResponse
                {
                    Result = RequestResult.Error,
                    Messages = new[] { ex.Message }
                };

                var response = context.Response;
                response.ContentType = "application/json";
                await response.WriteAsync(JsonSerializer.Serialize(result));
            }
            catch
            {
                throw;
            }
        }
    }
}
