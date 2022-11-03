using Funda.Core.QueueMessages;
using Funda.Queue.Abstractions;
using Funda.Queue.Worker.Console.Common;
using Microsoft.Extensions.Logging;

namespace Funda.Queue.Worker.Console;

internal class RealEstateAgentsWorker
{
    private readonly Worker<GetRealEstateAgent> _worker;
    private readonly GetRealEstateAgentMessageHandler _messageHandler;
    private readonly ILogger<RealEstateAgentsWorker> _logger;

    public RealEstateAgentsWorker(
        IQueue<GetRealEstateAgent> queue,
        GetRealEstateAgentMessageHandler messageHandler,
        ILogger<RealEstateAgentsWorker> logger)
    {
        _worker = new Worker<GetRealEstateAgent>(queue);
        _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Run(TimeSpan workerRunInterval)
    {
        _logger.LogInformation("Starting the worker...");

        await _worker.Run(
            message => _messageHandler.Handle(message).Result,
            workerRunInterval);
    }
}
