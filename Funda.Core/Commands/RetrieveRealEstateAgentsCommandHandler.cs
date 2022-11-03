using Funda.Common.CQRS.Abstractions;
using Funda.Core.QueueMessages;
using Funda.Queue.Abstractions;

namespace Funda.Core.Commands;

public class RetrieveRealEstateAgentsCommandHandler : ICommandHandler<RetrieveRealEstateAgentsCommand>
{
    private readonly IQueue<GetRealEstateAgent> _queue;

    public RetrieveRealEstateAgentsCommandHandler(IQueue<GetRealEstateAgent> queue) => 
        _queue = queue;

    public Task Handle(RetrieveRealEstateAgentsCommand command, CancellationToken cancellation)
    {
        var message = new GetRealEstateAgent
        {
            RetrievalId = command.RetrievalId,
            Location = command.Location,
            Outdoor = command.Outdoor,
            TopNumberOfAgents = command.TopNumberOfAgents,
        };
        _queue.Enqueue(message);
        return Task.CompletedTask;
    }
}
