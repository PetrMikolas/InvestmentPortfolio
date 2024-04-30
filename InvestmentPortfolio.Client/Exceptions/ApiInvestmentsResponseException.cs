namespace InvestmentPortfolio.Client.Exceptions;

/// <summary>
/// Represents an exception that is thrown when there is an error in the response received from the investments API.
/// </summary>
public class ApiInvestmentsResponseException : Exception
{
    public ApiInvestmentsResponseException() { }

    public ApiInvestmentsResponseException(string message) : base(message) { }

    public ApiInvestmentsResponseException(string message, Exception inner) : base(message, inner) { }
}