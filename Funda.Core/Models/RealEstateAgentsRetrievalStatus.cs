namespace Funda.Core.Models;

public class RealEstateAgentsRetrievalStatus
{
    public ProgressInfo? Progress { get; set; }
    public RealEstateAgent[]? RealEstateAgents { get; set; }
    public string? ErrorMessage { get; set; }
}
