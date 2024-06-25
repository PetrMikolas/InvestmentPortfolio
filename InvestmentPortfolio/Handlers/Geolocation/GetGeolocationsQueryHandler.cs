using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Queries.Geolocation;
using InvestmentPortfolio.Repositories.Geolocation;
using MediatR;

namespace InvestmentPortfolio.Handlers.Geolocation;

public sealed class GetGeolocationsQueryHandler(IGeolocationRepository repository, IMapper mapper) : IRequestHandler<GetGeolocationsQuery, IEnumerable<GeolocationDto>>
{
    public async Task<IEnumerable<GeolocationDto>> Handle(GetGeolocationsQuery request, CancellationToken cancellationToken)
    {
        var geolocations = await repository.GetAllAsync(cancellationToken);
        return mapper.Map<IEnumerable<GeolocationDto>>(geolocations);
    }
}