using GCCars.Application.Dto;
using GCCars.Application.Dto.Identity;
using GCCars.Application.Enums;
using GCCars.Application.Extensions;
using GCCars.Application.Models.Accounts;
using GCCars.Application.Services;
using GCCars.Application.Validators;
using GCCars.Application.Validators.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GCCars.Api.Controllers
{
    /// <summary>
    /// Аккаунты пользователей.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IdentityService identityService;
        private readonly IValidator<string> userAddressValidator;
        private readonly IValidator<decimal> topUpAccountValidator;
        private readonly IValidator<decimal> topDownAccountValidator;

        public AccountController(
            UserAddressValidator userAddressValidator,
            TopUpAccountValidator topUpAccountValidator,
            TopDownAccountValidator topDownAccountValidator,
            IdentityService identityService)
        {
            this.userAddressValidator = userAddressValidator;
            this.identityService = identityService;
            this.topUpAccountValidator = topUpAccountValidator;
            this.topDownAccountValidator = topDownAccountValidator;
        }

        /// <summary>
        /// Вход пользователя и получение токена доступа.
        /// </summary>
        /// <param name="address">Идентификатор пользователя.</param>
        /// <returns></returns>
        [HttpPost("token")]
        public async Task<ServerResponse<string>> GetToken([FromBody] GetTokenModel model)
        {
            if (!await userAddressValidator.Validate(model.Address).ConfigureAwait(false))
                return new ServerResponse<string>
                {
                    Result = RequestResult.Error,
                    Messages = userAddressValidator.GetErrors()
                };

            if (!await identityService.UserExists(model.Address).ConfigureAwait(false))
            {
                await identityService.RegisterUser(model.Address).ConfigureAwait(false);
            }
            await identityService.SignIn(model.Address).ConfigureAwait(false);
            var result = await identityService.GenerateToken(model.Address).ConfigureAwait(false);
            return new ServerResponse<string> { Data = result };
        }

        /// <summary>
        /// Пополнение баланса текущего пользователя.
        /// </summary>
        /// <param name="amount">Сумма пополнения.</param>
        /// <returns></returns>
        [HttpPost("top-up")]
        [Authorize]
        public async Task<ServerResponse<decimal>> TopUpAccount([FromBody] BalanceChangeModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            if (!await topUpAccountValidator.Validate(model.Amount, userId).ConfigureAwait(false))
                return new ServerResponse<decimal>
                {
                    Result = RequestResult.Error,
                    Messages = userAddressValidator.GetErrors()
                };

            var result = await identityService.TopUpAccount(userId, model.Amount).ConfigureAwait(false);
            return new ServerResponse<decimal> { Data = result };
        }
        
        /// <summary>
        /// Вывод с баланса текущего пользователя.
        /// </summary>
        /// <param name="amount">Сумма вывода.</param>
        /// <returns></returns>
        [HttpPost("withdraw")]
        [Authorize]
        public async Task<ServerResponse<decimal>> WithdrawAccount([FromBody] BalanceChangeModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            if (!await topDownAccountValidator.Validate(model.Amount, userId).ConfigureAwait(false))
                return new ServerResponse<decimal>
                {
                    Result = RequestResult.Error,
                    Messages = topDownAccountValidator.GetErrors()
                };

            var result = await identityService.WithdrawAccount(userId, model.Amount).ConfigureAwait(false);
            return new ServerResponse<decimal> { Data = result };
        }

        /// <summary>
        /// Получение информации о пользователе.
        /// </summary>
        /// <param name="address">Адрес пользователя.</param>
        /// <returns></returns>
        [HttpGet("userInfo")]
        [Authorize]
        public async Task<ServerResponse<UserInfoDto>> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            var result = await identityService.GetUserInfo(userId).ConfigureAwait(false);
            return new ServerResponse<UserInfoDto> { Data = result };
        }
    }
}
