using Funda.Common.Cqrs.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Common.Cqrs;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public Task Dispatch<TCommand>(TCommand command, CancellationToken cancellation)
        where TCommand : class, ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler.Handle(command, cancellation);
    }
}
