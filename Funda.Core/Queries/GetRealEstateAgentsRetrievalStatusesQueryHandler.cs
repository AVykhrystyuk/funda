using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;

namespace Funda.Core.Queries;

internal class GetRealEstateAgentsRetrievalStatusesQueryHandler :
    IQueryHandler<GetRealEstateAgentsRetrievalStatusesQuery, IReadOnlyList<RealEstateAgentsRetrievalStatus>>
{
    private readonly IDocumentCollection<RealEstateAgentsRetrievalStatus> _collection;

    public GetRealEstateAgentsRetrievalStatusesQueryHandler(IDocumentCollection<RealEstateAgentsRetrievalStatus> collection) =>
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));

    public async Task<IReadOnlyList<RealEstateAgentsRetrievalStatus>> Handle(
        GetRealEstateAgentsRetrievalStatusesQuery query,
        CancellationToken cancellation = default) => 
        await _collection.GetAll(cancellation);
}
