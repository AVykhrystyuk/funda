namespace Funda.Common.CQRS.Abstractions;

public interface ICommandDispatcher
{
    Task Dispatch<TCommand>(TCommand command, CancellationToken cancellation)
        where TCommand : class, ICommand;
}
