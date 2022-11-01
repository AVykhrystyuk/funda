using Funda.ApiClient.Abstractions.Models;
using Funda.Core.Models;

namespace Funda.Core;

public interface IRealEstateAgentsAggregator
{
    IReadOnlyList<RealEstateAgent> GetTopAgents(IEnumerable<RealEstateObject> objects, int numberOfAgents);
}
