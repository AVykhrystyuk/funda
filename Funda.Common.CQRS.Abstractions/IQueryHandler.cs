﻿namespace Funda.Common.CQRS.Abstractions;

public interface IQuery<TResult> { };

public interface IQueryHandler<in TQuery, TResult> : IQueryHandler<TResult>
    where TQuery : class, IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellation = default);

    Task<TResult> IQueryHandler<TResult>.Handle(object query, CancellationToken cancellation) 
        => Handle((TQuery)query, cancellation);
}

public interface IQueryHandler<TQueryResult>
{
    Task<TQueryResult> Handle(object query, CancellationToken cancellation = default);
}

