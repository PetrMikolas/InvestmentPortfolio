namespace InvestmentPortfolio.Repositories.Entities;

public class GeolocationEntity
{
    public int Id { get; set; }

    public string IpAddress { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string Isp { get; set; } = string.Empty;

    public string LocalDate { get; set; } = string.Empty;

    public string  Referer { get; set; } = string.Empty;
}