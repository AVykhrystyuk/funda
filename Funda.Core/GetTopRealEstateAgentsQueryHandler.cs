using Funda.ApiClient.Abstractions;
using Funda.ApiClient.Abstractions.Models;
using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Microsoft.Extensions.Logging;

namespace Funda.Core;

public record GetTopRealEstateAgentsQuery(
    string Location,
    string? Outdoor = null,
    int TopNumberOfAgents = 10) : IQuery<IReadOnlyList<RealEstateAgent>>;

internal class GetTopRealEstateAgentsQueryHandler : IQueryHandler<GetTopRealEstateAgentsQuery, IReadOnlyList<RealEstateAgent>>
{
    private readonly IFundaApiClient _fundaApiClient;
    private readonly ILogger<GetTopRealEstateAgentsQueryHandler> _logger;

    public GetTopRealEstateAgentsQueryHandler(IFundaApiClient fundaApiClient, ILogger<GetTopRealEstateAgentsQueryHandler> logger)
    {
        _fundaApiClient = fundaApiClient ?? throw new ArgumentNullException(nameof(fundaApiClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<RealEstateAgent>> Handle(GetTopRealEstateAgentsQuery query, CancellationToken cancellation = default)
    {
        var searchQuery = new SearchQuery(
            query.Location,
            query.Outdoor,
            SortBy.DateAscending); // NOTE: Important! Helps to avoid "Page drift" when new objects are added
        var pageRequest = new PageRequest(1);
        _logger.LogDebug("Retrieving real estate agents for {searchQuery} {page}", searchQuery, pageRequest);

        var response = await _fundaApiClient.GetRealEstateObjects(searchQuery, pageRequest, cancellation);
        if (response.TotalNumberOfObjects == 0)
            return Array.Empty<RealEstateAgent>();

        var allObjects = response.Objects.ToList();

        _logger.LogDebug("Found {total} objects, {count} pages to fetch", response.TotalNumberOfObjects, response.Paging.NumberOfPages);

        var requests = Enumerable.Range(2, response.Paging.NumberOfPages - 1)
            .Select(page => _fundaApiClient.GetRealEstateObjects(searchQuery, new PageRequest(page), cancellation));

        var responses = await Task.WhenAll(requests);
        allObjects.AddRange(responses.SelectMany(response => response.Objects));

        return allObjects
            .GroupBy(agent => new { agent.AgentId, agent.AgentName })
            .Select(group => new RealEstateAgent(group.Key.AgentId, group.Key.AgentName, ObjectCount: group.Count()))
            .OrderByDescending(agent => agent.ObjectCount)
            .Take(query.TopNumberOfAgents)
            .ToArray();
    }
}
