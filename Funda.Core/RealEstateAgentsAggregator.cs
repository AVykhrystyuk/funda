using Funda.ApiClient.Abstractions.Models;
using Funda.Core.Models;

namespace Funda.Core;

internal class RealEstateObjectsAggregator : IRealEstateObjectsAggregator
{
    public IReadOnlyList<RealEstateAgent> GetTopAgents(IEnumerable<RealEstateObject> objects, int numberOfAgents) =>
        objects
            .GroupBy(agent => new { agent.AgentId, agent.AgentName })
            .Select(group => new RealEstateAgent(group.Key.AgentId, group.Key.AgentName, ObjectCount: group.Count()))
            .OrderByDescending(agent => agent.ObjectCount)
            .Take(numberOfAgents)
            .ToArray();
}
