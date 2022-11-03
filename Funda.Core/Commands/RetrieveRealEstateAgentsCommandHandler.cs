using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Funda.Core.QueueMessages;
using Funda.DocumentStore.Abstractions;
using Funda.Queue.Abstractions;

namespace Funda.Core.Commands;

public class RetrieveRealEstateAgentsCommandHandler : ICommandHandler<RetrieveRealEstateAgentsCommand>
{
    private readonly IQueue<GetRealEstateAgent> _queue;
    private readonly IDocumentCollection<RealEstateAgentsRetrivalStatus> _collection;

    public RetrieveRealEstateAgentsCommandHandler(
        IQueue<GetRealEstateAgent> queue, 
        IDocumentCollection<RealEstateAgentsRetrivalStatus> collection)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public async Task Handle(RetrieveRealEstateAgentsCommand command, CancellationToken cancellation)
    {
        await _collection.Insert(command.RetrievalId, new RealEstateAgentsRetrivalStatus());

        var message = new GetRealEstateAgent
        {
            RetrievalId = command.RetrievalId,
            Location = command.Location,
            Outdoor = command.Outdoor,
            TopNumberOfAgents = command.TopNumberOfAgents,
        };
        await _queue.Enqueue(message);
    }
}
