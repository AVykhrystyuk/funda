namespace Funda.Web.Api.Swagger;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVersionedSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        // Add ApiExplorer to discover versions
        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}
