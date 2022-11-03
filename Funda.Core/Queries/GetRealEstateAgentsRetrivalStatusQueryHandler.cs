using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;

namespace Funda.Core.Queries;

internal class GetRealEstateAgentsRetrivalStatusQueryHandler :
    IQueryHandler<GetRealEstateAgentsRetrivalStatusQuery, RealEstateAgentsRetrivalStatus>
{
    private readonly IDocumentCollection<RealEstateAgentsRetrivalStatus> _collection;

    public GetRealEstateAgentsRetrivalStatusQueryHandler(IDocumentCollection<RealEstateAgentsRetrivalStatus> collection) =>
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));

    public async Task<RealEstateAgentsRetrivalStatus> Handle(
        GetRealEstateAgentsRetrivalStatusQuery query,
        CancellationToken cancellation = default) => 
        await _collection.Get(query.RetrivalId, cancellation);
}
