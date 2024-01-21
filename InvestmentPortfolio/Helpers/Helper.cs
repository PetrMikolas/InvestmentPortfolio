using InvestmentPortfolio.Models;

namespace InvestmentPortfolio.Helpers;

public static class Helper
{
    public static bool ValidateInvestmentDto(InvestmentDto investmentDto, out IResult? results)
    {
        if (investmentDto.Name.Length <= 0 || investmentDto.Name.Length > 40)
        {
            results = Results.BadRequest($"Parametr <{nameof(investmentDto.Name)}> musí mít délku 1 až 40 znaků");
            return false;
        }

        if (investmentDto.CurrencyCode.Length != 3)
        {
            results = Results.BadRequest($"Parametr <{nameof(investmentDto.CurrencyCode)}> musí mít délku 3 znaky");
            return false;
        }

        if (investmentDto.Value <= 0 || investmentDto.Value > 100000000)
        {
            results = Results.BadRequest($"Parametr <{nameof(investmentDto.Value)}> musí mít hodnotu v rozsahu 1 až 100 000 000");
            return false;
        }

        results = null;                        
        return true;
    }
}