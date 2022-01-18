using AutoMapper;
using GCCars.Application.Data;
using GCCars.Application.Dto.Identity;
using GCCars.Application.Exceptions;
using GCCars.Application.Extensions;
using GCCars.Application.Services.Base;
using GCCars.Application.Settings;
using GCCars.Domain.Constants;
using GCCars.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GCCars.Application.Services
{
    /// <summary>
    /// Сервис для работы с аккаунтами пользователей.
    /// </summary>
    public class IdentityService : BaseDbService
    {
        private readonly AuthSettings authSettings;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signinManager;

        public IdentityService(
            ILogger<IdentityService> logger,
            AppDbContext context,
            IMapper mapper,
            IOptions<AuthSettings> authSettings,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signinManager) : base(logger, context, mapper)
        {
            this.authSettings = authSettings.Value;
            this.userManager = userManager;
            this.signinManager = signinManager;
        }

        /// <summary>
        /// Генерация токена доступа пользователя.
        /// </summary>
        /// <param name="address">Идентификатор пользователя.</param>
        /// <returns>Строка токена.</returns>
        public async Task<string> GenerateToken(string address)
        {
            var user = await userManager.FindByNameAsync(address).ConfigureAwait(false);
            if (user == null)
                throw new Exception("User not found.");

            var claims = await userManager.GetClaimsAsync(user).ConfigureAwait(false);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: authSettings.Issuer,
                audience: authSettings.Audience,
                notBefore: now,
                claims: claims,
                expires: now.Add(TimeSpan.FromMinutes(authSettings.Lifetime)),
                signingCredentials: new SigningCredentials(authSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// Пополнение баланса пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="amount">Сумма пополнения.</param>
        /// <returns></returns>
        public async Task<decimal> TopUpAccount(int userId, decimal amount)
        {
            var user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            user.Balance += amount;
            await Db.SaveChangesAsync().ConfigureAwait(false);
            return user.Balance;
        }
        
        /// <summary>
        /// Вывод баланса пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="amount">Сумма вывода.</param>
        /// <returns></returns>
        public async Task<decimal> WithdrawAccount(int userId, decimal amount)
        {
            var user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            user.Balance -= amount;
            await Db.SaveChangesAsync().ConfigureAwait(false);
            return user.Balance;
        }

        /// <summary>
        /// Получение информации о пользователе.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns></returns>
        public async Task<UserInfoDto> GetUserInfo(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
            if (user == null)
                return null;
            var result = Mapper.Map<UserInfoDto>(user);
            result.CardBalance = await Db.Cars.CountAsync(x => x.OwnerId == user.Id & !x.IsDeleted);
            
            return result;
        }

        /// <summary>
        /// Получение пользователя по его идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns></returns>
        public async Task<AppUser> GetUser(int userId)
        {
            return await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Вход пользователя в приложение.
        /// </summary>
        /// <param name="address">Идентификатор пользователя.</param>
        /// <returns></returns>
        public async Task SignIn(string address)
        {
            var result = await signinManager.PasswordSignInAsync(address, address, false, false).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    throw new PublicException("Your account is blocked. Please contact support.");
                throw new PublicException("Wrong credential.");
            }
        }

        /// <summary>
        /// Регистрация пользователя в приложении.
        /// </summary>
        /// <param name="address">Идентификатор пользователя.</param>
        /// <returns></returns>
        public async Task RegisterUser(string address, string roleName = RoleName.CLIENT_USER)
        {
            using var tx = await Db.Database.BeginTransactionAsync().ConfigureAwait(false);

            var user = new AppUser
            {
                Email = address,
                UserName = address
            };
            var identityResult = await userManager.CreateAsync(user, address).ConfigureAwait(false);
            if (!identityResult.Succeeded)
                throw new PublicException(identityResult.Errors.GetErrorsMessage());

            identityResult = await userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);
            if (!identityResult.Succeeded)
                throw new PublicException(identityResult.Errors.GetErrorsMessage());

            await tx.CommitAsync();
        }

        /// <summary>
        /// Проверка регистрации пользователя.
        /// </summary>
        /// <param name="address">Идентификатор пользователя.</param>
        /// <returns></returns>
        public async Task<bool> UserExists(string address)
        {
            return await userManager.FindByNameAsync(address).ConfigureAwait(false) != null;
        }
    }
}
