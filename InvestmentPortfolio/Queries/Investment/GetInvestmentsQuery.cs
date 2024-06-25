using InvestmentPortfolio.Models;
using MediatR;

namespace InvestmentPortfolio.Queries.Investment;

public sealed record GetInvestmentsQuery(bool HasRefreshExchangeRates) : IRequest<InvestmentsDto>;