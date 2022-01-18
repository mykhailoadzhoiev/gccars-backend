using GCCars.Application.Dto;
using GCCars.Application.Enums;
using GCCars.Application.Extensions;
using GCCars.Application.Models.Transactions;
using GCCars.Application.Services;
using GCCars.Application.Validators;
using GCCars.Application.Validators.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GCCars.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly TransactionsService transactionsService;
        private readonly IValidator<StartTransactionModel> startTransactionValidator;
        private readonly IValidator<CompleteTransactionsModel> completeTransactionsValidator;

        public TransactionsController(
            TransactionsService transactionsService,
            StartTransactionValidator startTransactionValidator,
            CompleteTransactionsValidator completeTransactionsValidator)
        {
            this.transactionsService = transactionsService;
            this.startTransactionValidator = startTransactionValidator;
            this.completeTransactionsValidator = completeTransactionsValidator;
        }

        /// <summary>
        /// Начало отслеживания транзакции.
        /// </summary>
        /// <param name="model">Параметры транзакции.</param>
        /// <returns></returns>
        [HttpPost("start")]
        public async Task<ServerResponse> Start([FromBody] StartTransactionModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            if (!await startTransactionValidator.Validate(model, userId).ConfigureAwait(false))
                return new ServerResponse
                {
                    Messages = startTransactionValidator.GetErrors(),
                    Result = RequestResult.Error
                };

            await transactionsService.StartTransactionTracking(model.Address, model.StartTime).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Завершение отслеживания транзакций пользователя.
        /// </summary>
        /// <param name="model">Параметры транзакций.</param>
        /// <returns></returns>
        [HttpPost("complete")]
        public async Task<ServerResponse> Complete([FromBody] CompleteTransactionsModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            if (!await completeTransactionsValidator.Validate(model, userId).ConfigureAwait(false))
                return new ServerResponse
                {
                    Messages = completeTransactionsValidator.GetErrors(),
                    Result = RequestResult.Error
                };

            await transactionsService.CompleteTransactionsTracking(model.Address, model.StartTimes).ConfigureAwait(false);
            return ServerResponse.OK;
        }
    }
}
