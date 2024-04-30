using InvestmentPortfolio.Client.Exceptions;

namespace InvestmentPortfolio.Client.Services.Api;

/// <summary>
/// Represents a service for interacting with the investment API.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Retrieves investment data asynchronously from the API.
    /// </summary>
    /// <param name="hasRefresExchangeRates">Indicates whether to refresh exchange rates (optional). Defaults to <c>false</c>.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains investment data.</returns>
    /// <exception cref="ApiInvestmentsResponseException">Thrown when there is an error in the response received from the investments API.</exception>
    /// <exception cref="HttpRequestException">Thrown when there is an issue with the HTTP request, such as a network error.</exception>
    Task<InvestmentsDto> GetInvestmentsAsync(bool isRefresh = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a new investment asynchronously via the API.
    /// </summary>
    /// <param name="investment">The investment to save.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <exception cref="ApiInvestmentsResponseException">Thrown when there is an error in the response received from the investments API.</exception>
    /// <exception cref="HttpRequestException">Thrown when there is an issue with the HTTP request, such as a network error.</exception>
    Task SaveInvestmentAsync(InvestmentDto investment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing investment asynchronously via the API.
    /// </summary>
    /// <param name="investment">The updated investment data.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <exception cref="ApiInvestmentsResponseException">Thrown when there is an error in the response received from the investments API.</exception>
    /// <exception cref="HttpRequestException">Thrown when there is an issue with the HTTP request, such as a network error.</exception>
    Task UpdateInvestmentAsync(InvestmentDto investment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an investment asynchronously via the API.
    /// </summary>
    /// <param name="id">The ID of the investment to delete.</param>
    /// <param name="cancellationToken">The cancellation token (optional). Defaults to <see cref="CancellationToken.None"/>.</param>
    /// <exception cref="ApiInvestmentsResponseException">Thrown when there is an error in the response received from the investments API.</exception>
    /// <exception cref="HttpRequestException">Thrown when there is an issue with the HTTP request, such as a network error.</exception>
    Task DeleteInvestmentAsync(int id, CancellationToken cancellationToken = default);
}