using Funda.ApiClient.Abstractions.Models;
using Funda.Core.Models;

namespace Funda.Core;

public interface IRealEstateObjectsFetcher
{
    Task<IReadOnlyList<RealEstateObject>> Fetch(
        string location, 
        string[]? outdoors = null,
        Func<ProgressInfo, Task>? onProgress = null, 
        CancellationToken cancellation = default);
}
