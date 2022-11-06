using Funda.Common.Cqrs.Abstractions;

namespace Funda.Core.Commands;

public record RetrieveRealEstateAgentsCommand(
    Guid RetrievalId, 
    string Location,
    string[]? Outdoors, 
    int TopNumberOfAgents) : ICommand;
