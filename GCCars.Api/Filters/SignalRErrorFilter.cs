using GCCars.Application.Dto;
using GCCars.Application.Enums;
using GCCars.Application.Exceptions;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace GCCars.Api.Filters
{
    public class SignalRErrorFilter : IHubFilter
    {
        public async ValueTask<object> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            try
            {
                return await next(invocationContext);
            }
            catch (PublicException ex)
            {
                return new ValueTask<ServerResponse>(new ServerResponse { Result = RequestResult.Error, Messages = new[] { ex.Message } });
            }
            catch
            {
                throw;
            }
        }
    }
}
