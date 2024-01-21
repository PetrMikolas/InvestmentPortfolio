using System.Text.Json.Serialization;

namespace InvestmentPortfolio.Models;

public class GeolocationApi
{
    public int Id { get; set; }

    [JsonPropertyName("Query")]
    public string IpAddress { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string RegionName { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Zip { get; set; } = string.Empty;

    public double Lat { get; set; }

    public double Lon { get; set; }

    public string Timezone { get; set; } = string.Empty;

    public string Isp { get; set; } = string.Empty;

    public string Org { get; set; } = string.Empty;

    public string As { get; set; } = string.Empty;
}