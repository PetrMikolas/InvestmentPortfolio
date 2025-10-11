using InvestmentPortfolio.Models;
using MediatR;

namespace InvestmentPortfolio.Commands.Investment;

public sealed record CreateInvestmentCommand(InvestmentDto InvestmentDto) : IRequest<int>;