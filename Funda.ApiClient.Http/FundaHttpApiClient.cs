using System.Net;
using System.Net.Http.Json;
using Funda.ApiClient.Abstractions;
using Funda.ApiClient.Abstractions.Models;
using Funda.ApiClient.Http.Models;
using Funda.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Funda.ApiClient.Http;

public class FundaHttpApiClient : IFundaApiClient
{
    private readonly HttpClient _httpClient;
    private readonly FundaHttpApiOptions _options;
    private readonly ILogger<FundaHttpApiClient> _logger;

    public FundaHttpApiClient(HttpClient httpClient, IOptions<FundaHttpApiOptions> options, ILogger<FundaHttpApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public static bool IsTooManyRequests(HttpResponseMessage response) =>
        response.StatusCode == HttpStatusCode.Unauthorized; // HTTP 401 is thrown instead of HTTP 429 (Too Many Requests)

    public async Task<PagedResponse<RealEstateObject>> GetRealEstateObjects(
        SearchQuery query,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(page);

        var url = BuildUrl(query, page);
        _logger.LogDebug("Fetching data from {url}", url);

        var response = await _httpClient.GetFromJsonAsync<PagedResponseDto<RealEstateObjectDto>>(url, cancellationToken);
        return response!.ToPagedResponse(o => o.ToObject());
    }

    /// <returns> 
    /// [baseUrl]/[key]/?type=koop&zo=/[query]/&page=[page.Number]&pagesize=[page.PageSize]
    /// 
    /// Example:
    /// http://partnerapi.funda.nl/feeds/Aanbod.svc/json/[key]/?type=koop&zo=/amsterdam/tuin/sorteer-datum-op/&page=1&pagesize=25
    /// </returns>
    private string BuildUrl(SearchQuery query, PageRequest page)
    {
        var queryParams = string.Join("&", new[]
        {
            $"type=koop",
            $"zo=/{BuildSearchParams(query)}/",
            $"page={page.Number}",
            $"pagesize={Math.Min(page.PageSize, _options.MaxPageSize)}",
        });

        return string.Concat(
            _options.FeedsBaseUrl.EnsureEndsWith("/"),
            _options.ApiKey.EnsureEndsWith("/?"),
            queryParams);
    }

    private static string BuildSearchParams(SearchQuery query)
    {
        var searchParams = new[]
        {
            query.Location,
            query.Outdoor,
            SortByToUrlPart(query.SortBy),
        }.Where(p => !string.IsNullOrEmpty(p));

        return string.Join("/", searchParams);

        static string SortByToUrlPart(SortBy sortBy) => sortBy switch
        {
            SortBy.None => string.Empty,
            SortBy.DateAscending => "sorteer-datum-op",
            _ => throw new ArgumentOutOfRangeException(nameof(sortBy)),
        };
    }
}
