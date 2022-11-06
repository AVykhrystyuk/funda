namespace Funda.Common.Cqrs.Abstractions;

public interface IQueryDispatcher
{
    Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellation = default);
}
