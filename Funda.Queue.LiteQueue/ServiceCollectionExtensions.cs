using Funda.Queue.Abstractions;
using LiteDB;
using LiteQueue;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Queue.LiteQueue;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiteQueue<T>(
        this IServiceCollection services, 
        string queueCollection, 
        TimeSpan? dequeueInterval = null)
    {
        services.AddTransient<IQueue<T>>(provider =>
        {
            var db = provider.GetRequiredService<LiteDatabase>();
            var queue = new LiteQueue<T>(db, queueCollection);

            // Recommended on startup to reset anything that was checked out but not committed or aborted.
            queue.ResetOrphans();
            
            return new QueueAdapter<T>(queue, dequeueInterval);
        });

        return services;
    }
}
