namespace Funda.Core.Models;

public class RealEstateAgentsRetrivalStatus
{
    public ProgressInfo? Progress { get; set; }
    public RealEstateAgent[]? RealEstateAgents { get; set; }
    public string? ErrorMessage { get; set; }
}
