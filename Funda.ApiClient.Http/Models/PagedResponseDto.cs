using Funda.ApiClient.Abstractions.Models;

namespace Funda.ApiClient.Http.Models;

public record PagedResponseDto<T>(
    IReadOnlyList<T> Objects,
    PagingDto Paging,
    long TotaalAantalObjecten)
{
    public PagedResponse<TOut> ToPagedResponse<TOut>(Func<T, TOut> converter) => new(
        Objects: Objects.Select(converter).ToArray(),
        Paging: Paging.ToPaging(),
        TotalNumberOfObjects: TotaalAantalObjecten);
}
