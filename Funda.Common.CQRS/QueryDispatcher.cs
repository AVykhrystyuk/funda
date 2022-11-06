using Funda.Common.Cqrs.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Common.Cqrs;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(query);

        var handlerType = GenericHandlerType(query);
        var handler = (IQueryHandler<TResult>)_serviceProvider.GetRequiredService(handlerType);
        return handler.Handle(query, cancellation);
    }

    private static Type GenericHandlerType<TResult>(IQuery<TResult> query) =>
        typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));
}
