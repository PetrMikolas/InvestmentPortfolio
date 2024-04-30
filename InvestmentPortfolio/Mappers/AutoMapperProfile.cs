using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Repositories.Entities;

namespace InvestmentPortfolio.Mappers;

/// <summary>
/// Configures mappings between entities and models, models and DTOs, and DTOs and entities.
/// </summary>
public class AutoMapperProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.    
    /// </summary>
    public AutoMapperProfile()
    {
        CreateMap<InvestmentEntity, Investment>();
        CreateMap<Investments, InvestmentsDto>();
        CreateMap<InvestmentDto, InvestmentEntity>();
        CreateMap<GeolocationApi, GeolocationEntity>();
        CreateMap<GeolocationEntity, GeolocationDto>();
    }
}