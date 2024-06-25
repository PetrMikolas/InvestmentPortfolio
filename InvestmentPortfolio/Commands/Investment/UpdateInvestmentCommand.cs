using InvestmentPortfolio.Models;
using MediatR;

namespace InvestmentPortfolio.Commands.Investment;

public sealed record UpdateInvestmentCommand(InvestmentDto InvestmentDto) : IRequest;