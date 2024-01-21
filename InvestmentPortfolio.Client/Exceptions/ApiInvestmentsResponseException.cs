namespace InvestmentPortfolio.Client.Exceptions;

[Serializable]
public class ApiInvestmentsResponseException : Exception
{
    public ApiInvestmentsResponseException() { }

    public ApiInvestmentsResponseException(string message) : base(message) { }

    public ApiInvestmentsResponseException(string message, Exception inner) : base(message, inner) { }
}