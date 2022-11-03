using Funda.Queue.Abstractions;
using LiteDB;
using LiteQueue;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Queue.LiteQueue;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiteQueue<TMessage>(
        this IServiceCollection services,
        Action<LiteQueueOptions> configureOptions,
        TimeSpan? dequeueInterval = null)
    {
        var options = new LiteQueueOptions(); // services.AddOptions is not needed here
        configureOptions(options);

        return services
            .AddTransient(_ => new LiteDatabase(options.ConnectionString))
            .AddLiteQueue<TMessage>(options.Collection, dequeueInterval);
    }

    public static IServiceCollection AddLiteQueue<TMessage>(
        this IServiceCollection services,
        string queueCollection,
        TimeSpan? dequeueInterval = null) =>
        services.AddTransient<IQueue<TMessage>>(provider =>
        {
            var db = provider.GetRequiredService<LiteDatabase>();
            var queue = new LiteQueue<TMessage>(db, queueCollection);

            // Recommended on startup to reset anything that was checked out but not committed or aborted.
            queue.ResetOrphans();

            return new QueueAdapter<TMessage>(queue, dequeueInterval);
        });
}
