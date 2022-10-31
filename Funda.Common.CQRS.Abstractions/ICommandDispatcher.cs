namespace Funda.Common.CQRS.Abstractions;

public interface ICommandDispatcher
{
    Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellation);
}
