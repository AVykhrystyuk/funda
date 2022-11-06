namespace Funda.Common.Cqrs.Abstractions;

public interface ICommandHandler<in TCommand>
    where TCommand : class, ICommand
{
    Task Handle(TCommand command, CancellationToken cancellation);
}

