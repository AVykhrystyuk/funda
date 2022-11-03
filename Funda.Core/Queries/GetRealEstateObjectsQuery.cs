using Funda.ApiClient.Abstractions.Models;
using Funda.Common.CQRS.Abstractions;

namespace Funda.Core.Queries;

public record GetRealEstateObjectsQuery(
    string Location, 
    string? 
    Outdoor = null) : IQuery<IReadOnlyList<RealEstateObject>>;
