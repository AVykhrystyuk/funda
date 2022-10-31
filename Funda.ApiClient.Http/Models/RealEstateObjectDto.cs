using Funda.ApiClient.Abstractions.Models;

namespace Funda.ApiClient.Http.Models;

public record RealEstateObjectDto(long GlobalId, long MakelaarId, string MakelaarNaam)
{
    public RealEstateObject ToObject() => new(GlobalId, AgentId: MakelaarId, AgentName: MakelaarNaam);
}
