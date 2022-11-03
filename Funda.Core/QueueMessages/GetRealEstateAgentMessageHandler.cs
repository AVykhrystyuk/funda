using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;
using Microsoft.Extensions.Logging;

namespace Funda.Core.QueueMessages;

public class GetRealEstateAgentMessageHandler
{
    private readonly IRealEstateObjectsFetcher _objectsFetcher;
    private readonly IRealEstateObjectsAggregator _objectsAggregator;
    private readonly IDocumentCollection<RealEstateAgentsRetrivalStatus> _documentCollection;
    private readonly ILogger<GetRealEstateAgentMessageHandler> _logger;

    public GetRealEstateAgentMessageHandler(
        IRealEstateObjectsFetcher objectsFetcher,
        IRealEstateObjectsAggregator objectsAggregator,
        IDocumentCollection<RealEstateAgentsRetrivalStatus> documentCollection,
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
            await UpdateStatus(GetDocumentKey(message), status => status.ErrorMessage = ex.Message);
            return false;
        }

        static string LogKey(GetRealEstateAgent message) =>
            $"{message.RetrievalId}/{message.Location}/{message.Outdoor}/{message.TopNumberOfAgents}";
    }

    private async Task ProcessMessageInternal(GetRealEstateAgent message)
    {
        var key = GetDocumentKey(message);

        async Task OnProgress(ProgressInfo info) =>
            await UpdateStatus(key, status => status.Progress = info);

        var realEstateObjects = await _objectsFetcher.Fetch(message.Location, message.Outdoor, OnProgress);

        _logger.LogInformation("Aggregate top {NumberOfAgents} agents", message.TopNumberOfAgents);
        var realEstateAgents = _objectsAggregator.GetTopAgents(realEstateObjects, message.TopNumberOfAgents).ToArray();

        await UpdateStatus(key, status => status.RealEstateAgents = realEstateAgents);
    }

    private static string GetDocumentKey(GetRealEstateAgent message) => message.RetrievalId.ToString();

    private async Task UpdateStatus(string key, Func<RealEstateAgentsRetrivalStatus, RealEstateAgentsRetrivalStatus> updateStatus)
    {
        using var _ = _documentCollection.WriteLock();
        var status = await _documentCollection.Get(key);
        var updatedStatus = updateStatus(status);
        await _documentCollection.Update(key, status);
    }

    private Task UpdateStatus(string key, Action<RealEstateAgentsRetrivalStatus> updateStatus) =>
        UpdateStatus(key, status =>
        {
            updateStatus(status);
            return status;
        });
}
