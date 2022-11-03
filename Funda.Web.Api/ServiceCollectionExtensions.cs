using Funda.Core.QueueMessages;
using Funda.Queue.LiteQueue;
using LiteDB;

namespace Funda.Web.Api;

public static class ServiceCollectionExtensions
{
    public static void AddLiteDbWithQueue(this IServiceCollection services, Action<LiteDbOptions> configureLiteDbOptions)
    {
        var liteDbOptions = new LiteDbOptions(); // services.AddOptions is not needed here
        configureLiteDbOptions(liteDbOptions);

        services.AddTransient(_ => new LiteDatabase(liteDbOptions.ConnectionString));
        services.AddLiteQueue<GetRealEstateAgent>(liteDbOptions.QueueCollection);
    }
}
