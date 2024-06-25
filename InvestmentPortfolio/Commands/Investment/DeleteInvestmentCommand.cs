using MediatR;

namespace InvestmentPortfolio.Commands.Investment;

public sealed record DeleteInvestmentCommand(int Id) : IRequest;