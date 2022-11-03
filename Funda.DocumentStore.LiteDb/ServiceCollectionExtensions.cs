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
            .AddTransient(_ => new LiteDatabase(options.ConnectionString))
            .AddDocumentCollection<TDoc>(options.Collection);
    }

    public static IServiceCollection AddDocumentCollection<TDoc>(this IServiceCollection services, string name)
    {
        services.AddTransient<IDocumentCollection<TDoc>>(provider =>
        {
            var db = provider.GetRequiredService<LiteDatabase>();
            var collection = db.GetCollection<TDoc>(name);
            return new DocumentCollectionAdapter<TDoc>(db, collection);
        });

        return services;
    }
}
