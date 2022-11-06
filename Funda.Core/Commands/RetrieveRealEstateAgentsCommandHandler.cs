using Funda.Common.Cqrs.Abstractions;
using Funda.Core.Models;
using Funda.Core.QueueMessages;
using Funda.DocumentStore.Abstractions;
using Funda.Queue.Abstractions;

namespace Funda.Core.Commands;

public class RetrieveRealEstateAgentsCommandHandler : ICommandHandler<RetrieveRealEstateAgentsCommand>
{
    private readonly IQueue<GetRealEstateAgent> _queue;
    private readonly IDocumentCollection<RealEstateAgentsRetrieval> _collection;

    public RetrieveRealEstateAgentsCommandHandler(
        IQueue<GetRealEstateAgent> queue, 
        IDocumentCollection<RealEstateAgentsRetrieval> collection)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public async Task Handle(RetrieveRealEstateAgentsCommand command, CancellationToken cancellation)
    {
        var retrieval = new RealEstateAgentsRetrieval { RetrievalId = command.RetrievalId };
        await _collection.Insert(command.RetrievalId, retrieval);

        var message = new GetRealEstateAgent
        {
            RetrievalId = command.RetrievalId,
            Location = command.Location,
            Outdoors = command.Outdoors,
            TopNumberOfAgents = command.TopNumberOfAgents,
        };
        await _queue.Enqueue(message);
    }
}
