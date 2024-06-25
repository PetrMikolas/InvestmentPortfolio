using InvestmentPortfolio.Commands.Investment;
using InvestmentPortfolio.Services.Investment;
using MediatR;

namespace InvestmentPortfolio.Handlers.Investment;

public sealed class DeleteInvestmentCommandHandler(IInvestmentService investmentService) : IRequestHandler<DeleteInvestmentCommand>
{
    public async Task Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
    {
        await investmentService.DeleteAsync(request.Id, cancellationToken);
    }
}