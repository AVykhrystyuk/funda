using Funda.DocumentStore.Abstractions;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.DocumentStore.LiteDb;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiteDb<TDoc>(
        this IServiceCollection services,
        Action<LiteDbOptions> configureOptions)
    {
        var options = new LiteDbOptions(); // services.AddOptions is not needed here
        configureOptions(options);

        return services
            .AddTransient(_ => new LiteDatabaseContainer(new LiteDatabase(options.ConnectionString)))
            .AddDocumentCollection<TDoc>(options.Collection);
    }

    public static IServiceCollection AddDocumentCollection<TDoc>(this IServiceCollection services, string name) =>
        services.AddTransient<IDocumentCollection<TDoc>>(provider =>
        {
            var db = provider.GetRequiredService<LiteDatabaseContainer>().Db;
            var collection = db.GetCollection<TDoc>(name);
            return new DocumentCollectionAdapter<TDoc>(db, collection);
        });

    // local class to isolate LiteDatabase, to be able to re
    private record LiteDatabaseContainer(LiteDatabase Db);
}
