using AutoMapper;
using InvestmentPortfolio.Models;
using InvestmentPortfolio.Queries.Investment;
using InvestmentPortfolio.Services.Investment;
using MediatR;

namespace InvestmentPortfolio.Handlers.Investment;

public sealed class GetInvestmentsQueryHandler(IInvestmentService investmentService, IMapper mapper) : IRequestHandler<GetInvestmentsQuery, InvestmentsDto>
{
    public async Task<InvestmentsDto> Handle(GetInvestmentsQuery request, CancellationToken cancellationToken)
    {
        var investments = await investmentService.GetAllAsync(request.HasRefreshExchangeRates, cancellationToken);
        return mapper.Map<InvestmentsDto>(investments);
    }
}