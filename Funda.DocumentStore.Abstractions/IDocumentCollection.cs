namespace Funda.DocumentStore.Abstractions;

public interface IDocumentCollection<T>
{
    Task<IReadOnlyList<T>> GetAll(CancellationToken cancellation = default);
    Task<T> Get(string key, CancellationToken cancellation = default);
    Task Insert(string key, T entity);
    Task Update(string key, T entity);

    IDisposable WriteLock();
}

public static class DocumentCollectionExtensions
{
    public static Task<T> Get<T>(this IDocumentCollection<T> self, Guid key, CancellationToken cancellation = default) =>
        self.Get(key.ToString(), cancellation);

    public static Task Insert<T>(this IDocumentCollection<T> self, Guid key, T entity) =>
        self.Insert(key.ToString(), entity);

    public static Task Update<T>(this IDocumentCollection<T> self, Guid key, T entity) =>
        self.Update(key.ToString(), entity);
}
