using Funda.ApiClient.Abstractions.Models;

namespace Funda.ApiClient.Http.Models;

public record PagingDto(int AantalPaginas, int HuidigePagina)
{
    public Paging ToPaging() => new(CurrentPage: HuidigePagina, NumberOfPages: AantalPaginas);
}
