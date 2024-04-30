using System.Text.Json.Serialization;

namespace InvestmentPortfolio.Models;

/// <summary>
/// Represents geolocation information obtained from the Geolocation API.
/// </summary>
public sealed class GeolocationApi
{
    /// <summary>
    /// Gets or sets the unique identifier of the geolocation.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the IP address associated with the geolocation.
    /// </summary>
    [JsonPropertyName("Query")]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the geolocation request.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country name associated with the geolocation.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code associated with the geolocation.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region associated with the geolocation.
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the region associated with the geolocation.
    /// </summary>
    public string RegionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city associated with the geolocation.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ZIP code associated with the geolocation.
    /// </summary>
    public string Zip { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the latitude coordinate of the geolocation.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Gets or sets the longitude coordinate of the geolocation.
    /// </summary>
    public double Lon { get; set; }

    /// <summary>
    /// Gets or sets the timezone associated with the geolocation.
    /// </summary>
    public string Timezone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Internet Service Provider (ISP) associated with the geolocation.
    /// </summary>
    public string Isp { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the organization associated with the geolocation.
    /// </summary>
    public string Org { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the autonomous system (AS) associated with the geolocation.
    /// </summary>
    public string As { get; set; } = string.Empty;
}