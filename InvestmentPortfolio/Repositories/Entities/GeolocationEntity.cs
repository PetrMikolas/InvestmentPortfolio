namespace InvestmentPortfolio.Repositories.Entities;

/// <summary>
/// Represents the entity for geolocation information stored in the database.
/// </summary>
public class GeolocationEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the geolocation entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Represents the IP address associated with the geolocation entity.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Represents the city associated with the geolocation entity.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Represents the country associated with the geolocation entity.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Represents the ISP (Internet Service Provider) associated with the geolocation entity.
    /// </summary>
    public string Isp { get; set; } = string.Empty;

    /// <summary>
    /// Represents the local date associated with the geolocation entity.
    /// </summary>
    public string LocalDate { get; set; } = string.Empty;

    /// <summary>
    /// Represents the referer associated with the geolocation entity.
    /// </summary>
    public string Referer { get; set; } = string.Empty;
}