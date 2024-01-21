using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Mappers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<InvestmentEntity, Investment>();
        CreateMap<Investments, InvestmentsDto>();
        CreateMap<InvestmentDto, InvestmentEntity>();
        CreateMap<GeolocationApi, GeolocationEntity>();
        CreateMap<GeolocationEntity, GeolocationDto>();
    }
}