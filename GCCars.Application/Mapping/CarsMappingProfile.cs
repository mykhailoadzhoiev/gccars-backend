using AutoMapper;
using GCCars.Application.Dto.Cars;
using GCCars.Application.Dto.Dashboard;
using GCCars.Application.Dto.PvpBattles;
using GCCars.Domain.Enums;
using GCCars.Domain.Models.Cars;
using System.Linq;

namespace GCCars.Application.Mapping
{
    public class CarsMappingProfile : Profile
    {
        public CarsMappingProfile()
        {
            CreateMap<CarAttribute, Parameter>()
                .ForMember(d => d.CarParameter, o => o.MapFrom(s => s.Type))
                ;
            CreateMap<CarInfoDto, Car>()
                .ForMember(d => d.Parameters, o => o.MapFrom(s => s.Attributes))
                ;
            CreateMap<Parameter, ParameterDto>();
            CreateMap<Car, CarDto>()
                .ForMember(d => d.Address, o => o.MapFrom(s => s.Owner.UserName))
                .ForMember(d => d.Parameters, o => o.MapFrom(s => s.Parameters))
                ;
            CreateMap<Car, CarListItemDto>()
                .ForMember(d => d.Color, o => o.MapFrom(s => s.Parameters.First(r => r.CarParameter == CarParameter.Color).Value))
                .ForMember(d => d.OwnerAddress, o => o.MapFrom(s => s.Owner.UserName))
                ;
            CreateMap<Car, CarCardDto>()
                .IncludeBase<Car, CarDto>()
                ;
            CreateMap<Car, CarFighterDto>()
                .IncludeBase<Car, CarDto>()
                .ForMember(d => d.PvpBattleId, o => o.MapFrom(s => s.Fighters.First(r => r.PvpBattle.FinishedAt == null).PvpBattleId))
                ;
            CreateMap<Car, UserCarListItemDto>()
                .IncludeBase<Car, CarDto>()
                ;
        }
    }
}
