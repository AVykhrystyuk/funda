using Funda.ApiClient.Abstractions;
using Funda.ApiClient.Abstractions.Models;
using Microsoft.Extensions.Logging;
using Funda.Common;
using Funda.Core.Models;

namespace Funda.Core;

internal class RealEstateObjectsFetcher : IRealEstateObjectsFetcher
{
    private readonly IFundaApiClient _fundaApiClient;
    private readonly ILogger<RealEstateObjectsFetcher> _logger;

    public RealEstateObjectsFetcher(
        IFundaApiClient fundaApiClient,
        ILogger<RealEstateObjectsFetcher> logger)
    {
        _fundaApiClient = fundaApiClient ?? throw new ArgumentNullException(nameof(fundaApiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<RealEstateObject>> Fetch(
        string location,
        string[]? outdoors = null,
        Func<ProgressInfo, Task>? onProgress = null,
        CancellationToken cancellation = default)
    {
        var searchQuery = new SearchQuery(
            location,
            outdoors,
            SortBy.DateAscending); // NOTE: Important! Helps to avoid "Page drift" when new objects are added
        _logger.LogDebug("Start fetching real estate agents for {searchQuery}", searchQuery);

        var response = await FetchPage(searchQuery, new PageRequest(1), cancellation);
        if (response.TotalNumberOfObjects == 0)
            return Array.Empty<RealEstateObject>();

        long fetchedCount = response.Objects.Count;
        TrackProgress(onProgress, response, fetchedCount);

        var allObjects = response.Objects.ToList();

        _logger.LogDebug("Found {total} objects, {count} pages to fetch", response.TotalNumberOfObjects, response.Paging.NumberOfPages);

        var requests = Enumerable.Range(2, response.Paging.NumberOfPages - 1)
            .Select(async page => 
            {
                var response = await FetchPage(searchQuery, new PageRequest(page), cancellation);
                TrackProgress(
                    onProgress,
                    response,
                    fetchedCount: Interlocked.Add(ref fetchedCount, response.Objects.Count));
                return response;
            });

        var responses = await Task.WhenAll(requests);
        allObjects.AddRange(responses.SelectMany(response => response.Objects));

        return allObjects;
    }

    private static void TrackProgress(
        Func<ProgressInfo, Task>? onProgress, 
        PagedResponse<RealEstateObject> response,
        long fetchedCount)
    {
        if (onProgress is null) 
            return;
        onProgress(new ProgressInfo { Total = response.TotalNumberOfObjects, Fetched = fetchedCount })
            .Forget();
    }

    private async Task<PagedResponse<RealEstateObject>> FetchPage(
        SearchQuery searchQuery, 
        PageRequest pageRequest, 
        CancellationToken cancellation)
    {
        _logger.LogDebug("Retrieving real estate agents {page}", pageRequest);
        return await _fundaApiClient.GetRealEstateObjects(searchQuery, pageRequest, cancellation);
    }
}
