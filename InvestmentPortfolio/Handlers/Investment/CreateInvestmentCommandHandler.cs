using AutoMapper;
using InvestmentPortfolio.Repositories.Entities;
using InvestmentPortfolio.Services.Investment;
using MediatR;

namespace InvestmentPortfolio.Handlers.Investment;

public sealed class CreateInvestmentCommandHandler(IInvestmentService investmentService, IMapper mapper) : IRequestHandler<CreateInvestmentCommand, int>
{
    public async Task<int> Handle(CreateInvestmentCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<InvestmentEntity>(request.InvestmentDto);
        await investmentService.CreateAsync(entity, cancellationToken);
        return entity.Id;
    }
}