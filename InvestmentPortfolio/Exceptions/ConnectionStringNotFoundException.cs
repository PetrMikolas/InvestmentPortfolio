namespace InvestmentPortfolio.Exceptions;

[Serializable]
internal class ConnectionStringNotFoundException : Exception
{
    public ConnectionStringNotFoundException()
    {
    }

    public ConnectionStringNotFoundException(string? message) : base(message)
    {
    }

    public ConnectionStringNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}