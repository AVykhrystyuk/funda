namespace Funda.ApiClient.Abstractions.Models;

public record SearchQuery(string Location, string[]? Outdoors, SortBy SortBy = SortBy.None);
