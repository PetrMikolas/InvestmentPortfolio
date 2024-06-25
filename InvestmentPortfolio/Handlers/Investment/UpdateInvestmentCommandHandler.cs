using AutoMapper;
using InvestmentPortfolio.Commands.Investment;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Services.Investment;
using MediatR;

namespace InvestmentPortfolio.Handlers.Investment;

public sealed class UpdateInvestmentCommandHandler(IInvestmentService investmentService, IMapper mapper) : IRequestHandler<UpdateInvestmentCommand>
{
    public async Task Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<InvestmentEntity>(request.InvestmentDto);
        await investmentService.UpdateAsync(entity, cancellationToken);
    }
}