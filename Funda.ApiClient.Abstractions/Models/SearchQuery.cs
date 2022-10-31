namespace Funda.ApiClient.Abstractions.Models;

public record SearchQuery(string Location, string? Outdoor, SortBy SortBy = SortBy.None);
