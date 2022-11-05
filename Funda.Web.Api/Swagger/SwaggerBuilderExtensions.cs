using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Funda.Web.Api.Swagger;

public static class SwaggerBuilderExtensions
{
    public static IApplicationBuilder UseVersionedSwagger(this WebApplication app)
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger(options => 
        {
            //options.OperationFilter<SwaggerDefaultValues>());
        });

        app.UseSwaggerUI(options =>
        {
            AddSwaggerEndpoints(apiVersionDescriptionProvider, options);
            options.DisplayOperationId();
        });

        return app;
    }

    private static void AddSwaggerEndpoints(IApiVersionDescriptionProvider provider, SwaggerUIOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions.Reverse())
            options.SwaggerEndpoint(
                url: $"/swagger/{description.GroupName}/swagger.json",
                name: description.GroupName.ToUpperInvariant());
    }
}
