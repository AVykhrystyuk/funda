using Funda.ApiClient.Abstractions.Models;

namespace Funda.ApiClient.Abstractions;

public interface IFundaApiClient
{
    Task<PagedResponse<RealEstateObject>> GetRealEstateObjects(
        SearchQuery query,
        PageRequest page,
        CancellationToken cancellationToken = default);
}
