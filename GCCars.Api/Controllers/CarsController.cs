using GCCars.Application.Dto;
using GCCars.Application.Dto.Cars;
using GCCars.Application.Dto.PvpBattles;
using GCCars.Application.Extensions;
using GCCars.Application.Filters;
using GCCars.Application.Filters.Cars;
using GCCars.Application.Models.Cars;
using GCCars.Application.Services;
using GCCars.Application.Validators.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using GCCars.Application.Constants;
using GCCars.Application.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GCCars.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CarsController : Controller
    {
        private readonly CarsService carsService;
        private readonly IpfsService ipfsService;
        private readonly CarSaleValidator carSaleValidator;
        private readonly CarBuyValidator carBuyValidator;
        private readonly CarPushOutValidator carPushOutValidator;
        private readonly CarSaleCancelValidator carSaleCancelValidator;
        private readonly CarCardValidator carCardValidator;
        private readonly CarPutIntoValidator carPutIntoValidator;

        private readonly IHubContext<PvpBattlesHub> _hubContext;

        public CarsController(
            CarsService carsService,
            IpfsService ipfsService,
            CarSaleValidator carSaleValidator,
            CarBuyValidator carBuyValidator,
            CarPushOutValidator carPushOutValidator,
            CarSaleCancelValidator carSaleCancelValidator,
            CarCardValidator carCardValidator,
            CarPutIntoValidator carPutIntoValidator, IHubContext<PvpBattlesHub> hubContext)
        {
            this.carsService = carsService;
            this.ipfsService = ipfsService;
            this.carSaleValidator = carSaleValidator;
            this.carBuyValidator = carBuyValidator;
            this.carPushOutValidator = carPushOutValidator;
            this.carSaleCancelValidator = carSaleCancelValidator;
            this.carCardValidator = carCardValidator;
            this.carPutIntoValidator = carPutIntoValidator;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Получение данных одной машинки.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        [HttpGet("{carId:int}")]
        public async Task<ServerResponse<CarCardDto>> GetCard(int carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            if (!await carCardValidator.Validate(carId, userId).ConfigureAwait(false))
                return new ServerResponse<CarCardDto>
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = carCardValidator.GetErrors()
                };

            var result = await carsService.GetCard(carId).ConfigureAwait(false);
            return new ServerResponse<CarCardDto> { Data = result };
        }

        /// <summary>
        /// Получение списка машинок, выставленных на продажу.
        /// </summary>
        /// <returns></returns>
        [HttpPost("on-sale")]
        public async Task<ServerResponse<PagedData<CarDto>>> GetOnSale([FromBody] SearchFilter filter)
        {
            var result = await carsService.GetOnSale(filter).ConfigureAwait(false);
            return new ServerResponse<PagedData<CarDto>> { Data = result }; 
        }

        /// <summary>
        /// Получение списка машинок пользователя.
        /// </summary>
        /// <returns></returns>
        [HttpPost("user")]
        public async Task<ServerResponse<PagedData<UserCarListItemDto>>> GetList([FromBody] UserCarsFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt();
            var result = await carsService.GetUserCars(userId, filter.MintIds, filter.Skip, filter.Take, filter.Sorting).ConfigureAwait(false);
            return new ServerResponse<PagedData<UserCarListItemDto>> { Data = result };
        }

        /// <summary>
        /// Выставление машинки на продажу.
        /// </summary>
        /// <param name="model">Идентификатор машинки и цена продажи.</param>
        /// <returns></returns>
        [HttpPost("sell")]
        public async Task<ServerResponse> Sell([FromBody] CarSaleModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            if (!await carSaleValidator.Validate(model, userId).ConfigureAwait(false))
                return new ServerResponse<string>
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = carSaleValidator.GetErrors()
                };

            await carsService.Sell(model.CarId, model.Price).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Отмена продажи машинки.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        [HttpPost("{carId:int}/sale-cancel")]
        public async Task<ServerResponse> SaleCancel(int carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            if (!await carSaleCancelValidator.Validate(carId, userId).ConfigureAwait(false))
                return new ServerResponse<string>
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = carSaleCancelValidator.GetErrors()
                };

            await carsService.SaleCancel(carId).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Покупка машинки текущим пользователем.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        [HttpPost("{carId:int}/buy")]
        public async Task<ServerResponse> Buy(int carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            if (!await carBuyValidator.Validate(carId, userId).ConfigureAwait(false))
                return new ServerResponse<string>
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = carBuyValidator.GetErrors()
                };

            await carsService.Buy(carId, userId).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Вывод машинки текущего пользователя из платформы.
        /// </summary>
        /// <param name="carId">Идентификатор машинки.</param>
        /// <returns></returns>
        [HttpPost("{carId:int}/push-out")]
        public async Task<ServerResponse> PushOut(int carId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            if (!await carPushOutValidator.Validate(carId, userId).ConfigureAwait(false))
                return new ServerResponse<string>
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = carPushOutValidator.GetErrors()
                };

            await carsService.PushOut(carId).ConfigureAwait(false);
            return ServerResponse.OK;
        }

        /// <summary>
        /// Заведение машинки на платформе текущим пользователем.
        /// </summary>
        /// <param name="jsonPath">Путь до файла с JSON машинки.</param>
        /// <returns></returns>
        [HttpPost("{carId:int}/put-into")]
        public async Task<ServerResponse<int>> PutInto(int carId)
        {
            if (!await carPutIntoValidator.Validate(carId, null).ConfigureAwait(false))
                return new ServerResponse<int>
                {
                    Result = Application.Enums.RequestResult.Error,
                    Messages = carPutIntoValidator.GetErrors()
                };

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToInt().Value;
            var result = await carsService.PutInto(carId, userId).ConfigureAwait(false);
            return new ServerResponse<int> { Data = result };
        }

        // [HttpGet("test1")]
        // [AllowAnonymous]
        // public async Task<IActionResult> Test1()
        // {
        //     await _hubContext.Clients.All.SendAsync(HubMethod.UPDATE_PVP_BATTLES_LIST).ConfigureAwait(false);
        //     return Ok();
        // }
        //
        // [HttpGet("test2")]
        // [AllowAnonymous]
        // public async Task<IActionResult> Test2()
        // {
        //     await _hubContext.Clients.All.SendAsync(HubMethod.UPDATE_PVP_BATTLE, 19).ConfigureAwait(false);
        //     return Ok();
        // }
    }
}
