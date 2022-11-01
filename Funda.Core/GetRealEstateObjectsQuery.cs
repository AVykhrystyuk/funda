using Funda.ApiClient.Abstractions.Models;
using Funda.Common.CQRS.Abstractions;

namespace Funda.Core;

public record GetRealEstateObjectsQuery(string Location, string? Outdoor = null) : IQuery<IReadOnlyList<RealEstateObject>>;
