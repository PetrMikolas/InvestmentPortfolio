namespace InvestmentPortfolio.Repositories.Entities;

public class InvestmentEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Value { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public long? DefaultValueCzk { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? ModifiedDate { get; set; }   
}