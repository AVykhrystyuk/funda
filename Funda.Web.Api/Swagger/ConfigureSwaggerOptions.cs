using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Funda.Web.Api.Swagger;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => 
        _provider = provider;

    public void Configure(string name, SwaggerGenOptions options) => 
        Configure(options);

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                name: description.GroupName,
                info: CreateVersionInfo(description));
        }

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        options.CustomOperationIds((apiDescription) => apiDescription.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
    }

    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
    {
        var info = new OpenApiInfo
        {
            Title = "Funda Web API",
            Version = desc.ApiVersion.ToString(),
            Description = "Helps to determine which real estate agent have the most object listed for sale.",
        };

        if (desc.IsDeprecated)
        {
            info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
        }

        return info;
    }
}
