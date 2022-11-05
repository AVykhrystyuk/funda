using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;
using Microsoft.Extensions.Logging;

namespace Funda.Core.QueueMessages;

public class GetRealEstateAgentMessageHandler
{
    private readonly IRealEstateObjectsFetcher _objectsFetcher;
    private readonly IRealEstateObjectsAggregator _objectsAggregator;
    private readonly IDocumentCollection<RealEstateAgentsRetrieval> _documentCollection;
    private readonly ILogger<GetRealEstateAgentMessageHandler> _logger;

    public GetRealEstateAgentMessageHandler(
        IRealEstateObjectsFetcher objectsFetcher,
        IRealEstateObjectsAggregator objectsAggregator,
        IDocumentCollection<RealEstateAgentsRetrieval> documentCollection,
        ILogger<GetRealEstateAgentMessageHandler> logger)
    {
        _objectsFetcher = objectsFetcher ?? throw new ArgumentNullException(nameof(objectsFetcher));
        _objectsAggregator = objectsAggregator ?? throw new ArgumentNullException(nameof(objectsAggregator));
        _documentCollection = documentCollection ?? throw new ArgumentNullException(nameof(documentCollection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(GetRealEstateAgent message)
    {
        var logKey = LogKey(message);
        _logger.LogInformation("Attempt to process the message {Key}", logKey);

        try
        {
            await ProcessMessageInternal(message);

            _logger.LogInformation("Sucessfully processed the message {Key}", logKey);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing the message {Key}", logKey);
            await UpdateRetrieval(GetDocumentKey(message), retrieval => retrieval.ErrorMessage = ex.Message);
            return false;
        }

        static string LogKey(GetRealEstateAgent message) =>
            $"{message.RetrievalId}/{message.Location}/{message.Outdoor}/{message.TopNumberOfAgents}";
    }

    private async Task ProcessMessageInternal(GetRealEstateAgent message)
    {
        var key = GetDocumentKey(message);

        async Task OnProgress(ProgressInfo info) =>
            await UpdateRetrieval(key, retrieval => retrieval.Progress = info);

        var realEstateObjects = await _objectsFetcher.Fetch(message.Location, message.Outdoor, OnProgress);

        _logger.LogInformation("Aggregate top {NumberOfAgents} agents", message.TopNumberOfAgents);
        var realEstateAgents = _objectsAggregator.GetTopAgents(realEstateObjects, message.TopNumberOfAgents).ToArray();

        await UpdateRetrieval(key, retrieval => retrieval.RealEstateAgents = realEstateAgents);
    }

    private static string GetDocumentKey(GetRealEstateAgent message) => message.RetrievalId.ToString();

    private async Task UpdateRetrieval(string key, Func<RealEstateAgentsRetrieval, RealEstateAgentsRetrieval> updateRetrieval)
    {
        using var _ = _documentCollection.WriteLock();
        var retrieval = await _documentCollection.Get(key);
        var updatedRetrieval = updateRetrieval(retrieval);
        await _documentCollection.Update(key, updatedRetrieval);
    }

    private Task UpdateRetrieval(string key, Action<RealEstateAgentsRetrieval> updateRetrieval) =>
        UpdateRetrieval(key, retrieval =>
        {
            updateRetrieval(retrieval);
            return retrieval;
        });
}
