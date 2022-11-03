using Funda.DocumentStore.Abstractions;
using LiteDB;

namespace Funda.DocumentStore.LiteDb;

public class DocumentCollectionAdapter<T> : IDocumentCollection<T>
{
    private readonly LiteDatabase _database;
    private readonly LiteCollection<T> _collection;

    public DocumentCollectionAdapter(LiteDatabase database, LiteCollection<T> collection)
    {
        _database = database ?? throw new ArgumentNullException(nameof(collection));
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public IDisposable WriteLock() => 
        _database.Engine.Locker.Write();

    public Task<T> Get(string key, CancellationToken cancellation) =>
        Task.FromResult(_collection.FindById(key));

    public Task Insert(string key, T entity)
    {
        _collection.Insert(key, entity);
        return Task.CompletedTask;
    }

    public Task Update(string key, T entity)
    {
        _collection.Update(key, entity);
        return Task.CompletedTask;
    }
}
