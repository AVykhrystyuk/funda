using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;

namespace Funda.Core.Queries;

internal class GetRealEstateAgentsRetrievalQueryHandler :
    IQueryHandler<GetRealEstateAgentsRetrievalQuery, RealEstateAgentsRetrieval>
{
    private readonly IDocumentCollection<RealEstateAgentsRetrieval> _collection;

    public GetRealEstateAgentsRetrievalQueryHandler(IDocumentCollection<RealEstateAgentsRetrieval> collection) =>
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));

    public async Task<RealEstateAgentsRetrieval> Handle(
        GetRealEstateAgentsRetrievalQuery query,
        CancellationToken cancellation = default) => 
        await _collection.Get(query.RetrievalId, cancellation);
}
