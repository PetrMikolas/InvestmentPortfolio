namespace InvestmentPortfolio.Exceptions;

/// <summary>
/// Represents errors that occur when an entity is not found.
/// </summary>
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() { }

    public EntityNotFoundException(string message) : base(message) { }

    public EntityNotFoundException(string message, Exception inner) : base(message, inner) { }
}