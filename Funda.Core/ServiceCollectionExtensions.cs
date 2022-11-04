using Funda.Common.CQRS;
using Funda.Common.CQRS.Abstractions;
using Funda.Core.Queries;
using Funda.Core.QueueMessages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Funda.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services) =>
        services.AddCqrs()
            .AddSingleton<IRealEstateObjectsAggregator, RealEstateObjectsAggregator>()
            .AddSingleton<IRealEstateObjectsFetcher, RealEstateObjectsFetcher>()
            .AddSingleton<GetRealEstateAgentMessageHandler>();

    private static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.TryAddSingleton<IQueryDispatcher, QueryDispatcher>();

        var assembly = typeof(GetRealEstateAgentsRetrievalStatusQuery).Assembly; // contains Query/Command handlers

        // Using https://www.nuget.org/packages/Scrutor for registering all Query and Command handlers by convention
        return services
            .Scan(selector =>
            {
                selector.FromAssemblies(assembly)
                    .AddClasses(filter => filter.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
            })
            .Scan(selector =>
            {
                selector.FromAssemblies(assembly)
                    .AddClasses(filter => filter.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
            });
    }
}
