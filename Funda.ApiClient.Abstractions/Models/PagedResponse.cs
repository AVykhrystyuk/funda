namespace Funda.ApiClient.Abstractions.Models;

public record PagedResponse<T>(
    IReadOnlyList<T> Objects,
    Paging Paging,
    long TotalNumberOfObjects);
