using Radzen;

namespace InvestmentPortfolio.Client.Models;

public sealed class AlertMessage
{
    public string Text { get; set; } = string.Empty;

    public AlertStyle Style { get; set; }  
}