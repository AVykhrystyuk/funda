namespace Funda.Common.CQRS.Abstractions;

public interface ICommand<TResult> { };

public interface ICommandHandler<in TCommand, TResult> : ICommandHandler<TResult>
    where TCommand : class, ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellation);

    Task<TResult> ICommandHandler<TResult>.Handle(object query, CancellationToken cancellation) 
        => Handle((TCommand)query, cancellation);
}

public interface ICommandHandler<TResult>
{
    Task<TResult> Handle(object query, CancellationToken cancellation = default);
}
