using InvestmentPortfolio.Models;
using MediatR;

public sealed record CreateInvestmentCommand(InvestmentDto InvestmentDto) : IRequest<int>;