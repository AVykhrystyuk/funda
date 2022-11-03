namespace Funda.Common.CQRS.Abstractions;

public interface IQueryDispatcher
{
    Task<TResult> Dispatch<TResult>(IQuery<TResult> query, CancellationToken cancellation = default);
}
