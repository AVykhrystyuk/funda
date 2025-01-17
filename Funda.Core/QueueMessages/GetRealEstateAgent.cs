﻿namespace Funda.Core.QueueMessages;

public record GetRealEstateAgent
{
    public Guid RetrievalId { get; init; }
    public string Location { get; init; } = string.Empty;
    public string[]? Outdoors { get; init; }
    public int TopNumberOfAgents { get; init; }
}

