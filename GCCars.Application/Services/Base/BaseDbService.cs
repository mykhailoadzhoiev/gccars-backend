using AutoMapper;
using GCCars.Application.Data;
using Microsoft.Extensions.Logging;

namespace GCCars.Application.Services.Base
{
    /// <summary>
    /// Базовый класс для сервисов с использованием БД.
    /// </summary>
    public class BaseDbService
    {
        protected readonly ILogger Logger;
        protected readonly AppDbContext Db;
        protected readonly IMapper Mapper;

        public BaseDbService(
            ILogger logger,
            AppDbContext context,
            IMapper mapper)
        {
            Logger = logger;
            Db = context;
            Mapper = mapper;
        }
    }
}
