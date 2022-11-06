using Funda.Common.Cqrs.Abstractions;
using Funda.Core.Models;

namespace Funda.Core.Queries;

public record GetRealEstateAgentsRetrievalQuery(Guid RetrievalId) : IQuery<RealEstateAgentsRetrieval>;
