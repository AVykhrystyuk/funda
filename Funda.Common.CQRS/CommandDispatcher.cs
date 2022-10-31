using Funda.Common.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Common.CQRS;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(command);

        var handlerType = GenericHandlerType(command);
        var handler = (ICommandHandler<TResult>)_serviceProvider.GetRequiredService(handlerType);
        return handler.Handle(command, cancellation);
    }

    private static Type GenericHandlerType<TResult>(ICommand<TResult> command) =>
        typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResult));
}
