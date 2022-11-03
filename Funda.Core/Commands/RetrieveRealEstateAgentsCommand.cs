using Funda.Common.CQRS.Abstractions;

namespace Funda.Core.Commands;

public record RetrieveRealEstateAgentsCommand(
    Guid RetrievalId, 
    string Location, 
    string? Outdoor, 
    int TopNumberOfAgents) : ICommand;
