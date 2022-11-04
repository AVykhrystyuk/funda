using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;

namespace Funda.Core.Queries;

internal class GetRealEstateAgentsRetrievalStatusQueryHandler :
    IQueryHandler<GetRealEstateAgentsRetrievalStatusQuery, RealEstateAgentsRetrievalStatus>
{
    private readonly IDocumentCollection<RealEstateAgentsRetrievalStatus> _collection;

    public GetRealEstateAgentsRetrievalStatusQueryHandler(IDocumentCollection<RealEstateAgentsRetrievalStatus> collection) =>
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));

    public async Task<RealEstateAgentsRetrievalStatus> Handle(
        GetRealEstateAgentsRetrievalStatusQuery query,
        CancellationToken cancellation = default) => 
        await _collection.Get(query.RetrievalId, cancellation);
}
