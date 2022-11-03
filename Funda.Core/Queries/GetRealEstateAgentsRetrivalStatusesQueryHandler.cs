using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;
using Funda.DocumentStore.Abstractions;

namespace Funda.Core.Queries;

internal class GetRealEstateAgentsRetrivalStatusesQueryHandler :
    IQueryHandler<GetRealEstateAgentsRetrivalStatusesQuery, IReadOnlyList<RealEstateAgentsRetrivalStatus>>
{
    private readonly IDocumentCollection<RealEstateAgentsRetrivalStatus> _collection;

    public GetRealEstateAgentsRetrivalStatusesQueryHandler(IDocumentCollection<RealEstateAgentsRetrivalStatus> collection) =>
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));

    public async Task<IReadOnlyList<RealEstateAgentsRetrivalStatus>> Handle(
        GetRealEstateAgentsRetrivalStatusesQuery query,
        CancellationToken cancellation = default) => 
        await _collection.GetAll(cancellation);
}
