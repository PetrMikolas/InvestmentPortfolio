using InvestmentPortfolio.Models;

namespace InvestmentPortfolio.Helpers;

public static class Helper
{
    public static bool IsValidInvestmentDto(InvestmentDto investmentDto, out string error)
    {
        error = string.Empty;

        if (investmentDto.Name.Length <= 0 || investmentDto.Name.Length > 40)
        {
            error = $"Parametr <{nameof(investmentDto.Name)}> musí mít délku 1 až 40 znaků. ";
        }

        if (investmentDto.CurrencyCode.Length != 3)
        {
            error += $"Parametr <{nameof(investmentDto.CurrencyCode)}> musí mít délku 3 znaky. ";
        }

        if (investmentDto.Value <= 0 || investmentDto.Value > 100000000)
        {
            error += $"Parametr <{nameof(investmentDto.Value)}> musí mít hodnotu v rozsahu 1 až 100 000 000.";
        }

        return error == string.Empty;
    }
}