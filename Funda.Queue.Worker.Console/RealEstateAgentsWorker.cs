using Funda.Common.CQRS.Abstractions;
using Funda.Core;
using Funda.Core.Queries;
using Funda.Core.QueueMessages;
using Funda.Queue.Abstractions;
using Funda.Queue.Worker.Console.Common;
using Microsoft.Extensions.Logging;

namespace Funda.Queue.Worker.Console;

internal class RealEstateAgentsWorker
{
    private readonly Worker<GetRealEstateAgent> _worker;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IRealEstateObjectsAggregator _objectsAggregator;
    private readonly ILogger<RealEstateAgentsWorker> _logger;

    public RealEstateAgentsWorker(
        IQueue<GetRealEstateAgent> queue,
        IQueryDispatcher queryDispatcher,
        IRealEstateObjectsAggregator objectsAggregator,
        ILogger<RealEstateAgentsWorker> logger)
    {
        _worker = new Worker<GetRealEstateAgent>(queue);
        _queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
        _objectsAggregator = objectsAggregator ?? throw new ArgumentNullException(nameof(objectsAggregator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Run(TimeSpan workerRunInterval)
    {
        _logger.LogInformation("Starting the worker...");

        _worker.Run(
            message => ProcessMessage(message).Result,
            workerRunInterval);
    }

    private async Task<bool> ProcessMessage(GetRealEstateAgent message)
    {
        _logger.LogInformation("Attempt to process the message {Key}", Key(message));

        try
        {
            var queue = new GetRealEstateObjectsQuery(message.Location, message.Outdoor);
            var realEstateObjects = await _queryDispatcher.Dispatch(queue);
            var realEstateAgents = _objectsAggregator.GetTopAgents(realEstateObjects, message.TopNumberOfAgents);

            _logger.LogInformation("Sucessfully processed the message {Key}", Key(message));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing the message {Key}", Key(message));

            return false;
        }

        static string Key(GetRealEstateAgent message) => 
            $"{message.RetrievalId}/{message.Location}/{message.Outdoor}/{message.TopNumberOfAgents}";
    }
}
