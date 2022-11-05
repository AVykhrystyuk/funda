namespace Funda.Core.Models;

public class RealEstateAgentsRetrieval
{
    public Guid RetrievalId { get; set; }
    public ProgressInfo? Progress { get; set; }
    public RealEstateAgent[]? RealEstateAgents { get; set; }
    public string? ErrorMessage { get; set; }
}
