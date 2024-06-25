using InvestmentPortfolio.Models;
using MediatR;

namespace InvestmentPortfolio.Queries.Geolocation;

public sealed record GetGeolocationsQuery() : IRequest<IEnumerable<GeolocationDto>>;