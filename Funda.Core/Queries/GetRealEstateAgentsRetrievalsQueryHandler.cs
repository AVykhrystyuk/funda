using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;

namespace Funda.Core.Queries;

internal class GetRealEstateAgentsRetrievalsQueryHandler :
    IQueryHandler<GetRealEstateAgentsRetrievalsQuery, IReadOnlyList<RealEstateAgentsRetrieval>>
{
    private readonly IDocumentCollection<RealEstateAgentsRetrieval> _collection;

    public GetRealEstateAgentsRetrievalsQueryHandler(IDocumentCollection<RealEstateAgentsRetrieval> collection) =>
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));

    public async Task<IReadOnlyList<RealEstateAgentsRetrieval>> Handle(
        GetRealEstateAgentsRetrievalsQuery query,
        CancellationToken cancellation = default) => 
        await _collection.GetAll(cancellation);
}
