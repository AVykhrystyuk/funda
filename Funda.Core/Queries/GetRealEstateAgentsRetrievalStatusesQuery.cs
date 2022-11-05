﻿using Funda.Common.CQRS.Abstractions;
using Funda.Core.Models;

namespace Funda.Core.Queries;

public record GetRealEstateAgentsRetrievalStatusesQuery() : IQuery<IReadOnlyList<RealEstateAgentsRetrievalStatus>>;