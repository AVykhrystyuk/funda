using Funda.Common.Cqrs.Abstractions;
using Funda.Core.Models;

namespace Funda.Core.Queries;

public record GetRealEstateAgentsRetrievalsQuery() : IQuery<IReadOnlyList<RealEstateAgentsRetrieval>>;
