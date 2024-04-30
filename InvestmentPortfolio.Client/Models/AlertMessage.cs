using Radzen;

namespace InvestmentPortfolio.Client.Models;

/// <summary>
/// Represents a message to display in an alert component, including text and style.
/// </summary>
public sealed class AlertMessage
{
    /// <summary>
    /// Gets or sets the text content of the alert message.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the style of the alert message, represented by an enum value.
    /// </summary>
    public AlertStyle Style { get; set; }
}