using Funda.ApiClient.Abstractions;
using Funda.ApiClient.Abstractions.Models;
using Funda.Common.CQRS.Abstractions;
using Microsoft.Extensions.Logging;

namespace Funda.Core.Queries;

internal class GetRealEstateObjectsQueryHandler : IQueryHandler<GetRealEstateObjectsQuery, IReadOnlyList<RealEstateObject>>
{
    private readonly IFundaApiClient _fundaApiClient;
    private readonly ILogger<GetRealEstateObjectsQueryHandler> _logger;

    public GetRealEstateObjectsQueryHandler(IFundaApiClient fundaApiClient, ILogger<GetRealEstateObjectsQueryHandler> logger)
    {
        _fundaApiClient = fundaApiClient ?? throw new ArgumentNullException(nameof(fundaApiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<RealEstateObject>> Handle(
        GetRealEstateObjectsQuery query,
        CancellationToken cancellation = default)
    {
        var searchQuery = new SearchQuery(
            query.Location,
            query.Outdoor,
            SortBy.DateAscending); // NOTE: Important! Helps to avoid "Page drift" when new objects are added
        var pageRequest = new PageRequest(1);
        _logger.LogDebug("Retrieving real estate agents for {searchQuery} {page}", searchQuery, pageRequest);

        var response = await _fundaApiClient.GetRealEstateObjects(searchQuery, pageRequest, cancellation);
        if (response.TotalNumberOfObjects == 0)
            return Array.Empty<RealEstateObject>();

        var allObjects = response.Objects.ToList();

        _logger.LogDebug("Found {total} objects, {count} pages to fetch", response.TotalNumberOfObjects, response.Paging.NumberOfPages);

        var requests = Enumerable.Range(2, response.Paging.NumberOfPages - 1)
            .Select(page => _fundaApiClient.GetRealEstateObjects(searchQuery, new PageRequest(page), cancellation));

        var responses = await Task.WhenAll(requests);
        allObjects.AddRange(responses.SelectMany(response => response.Objects));

        return allObjects;
    }
}
