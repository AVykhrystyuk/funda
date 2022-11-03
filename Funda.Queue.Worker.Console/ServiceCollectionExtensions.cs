using Funda.Core.QueueMessages;
using Funda.Queue.LiteQueue;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Queue.Worker.Console;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiteDbWithQueue(
        this IServiceCollection services, 
        Action<LiteDbOptions> configureLiteDbOptions, 
        TimeSpan? dequeueInterval = null)
    {
        var liteDbOptions = new LiteDbOptions(); // services.AddOptions is not needed here
        configureLiteDbOptions(liteDbOptions);

        services.AddTransient(_ => new LiteDatabase(liteDbOptions.ConnectionString));
        services.AddLiteQueue<GetRealEstateAgent>(liteDbOptions.QueueCollection, dequeueInterval);

        return services;
    }
}
