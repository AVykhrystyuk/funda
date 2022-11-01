using Funda.Common.CQRS;
using Funda.Common.CQRS.Abstractions;
using Funda.Core.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Funda.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddCqrs();
        services.AddSingleton<IRealEstateAgentsAggregator, RealEstateAgentsAggregator>();
        return services;
    }

    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandDispatcher, CommandDispatcher>();
        services.TryAddSingleton<IQueryDispatcher, QueryDispatcher>();

        var assembly = typeof(GetRealEstateObjectsQuery).Assembly;

        // INFO: Using https://www.nuget.org/packages/Scrutor for registering all Query and Command handlers by convention
        services.Scan(selector =>
        {
            selector.FromAssemblies(assembly)
                .AddClasses(filter => filter.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime();
        });
        services.Scan(selector =>
        {
            selector.FromAssemblies(assembly)
                .AddClasses(filter => filter.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime();
        });

        return services;
    }
}
