namespace InvestmentPortfolio.Services.Email;

public interface IEmailService
{
    void SendError(string errorMessage);

    void SendError(string errorMessage, Type classType, string methodName);

    void SendObject<TValue>(TValue value, string subject) where TValue : class;
}