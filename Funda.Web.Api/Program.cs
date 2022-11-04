using System.Text.Json.Serialization;
using Funda.ApiClient.Http;
using Funda.Core;
using Funda.Core.Models;
using Funda.Core.QueueMessages;
using Funda.DocumentStore.LiteDb;
using Funda.Queue.LiteQueue;
using Funda.Web.Api.Swagger;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// builder.Services.AddEndpointsApiExplorer(); // 'minimal API' is not used https://stackoverflow.com/questions/71932980/what-is-addendpointsapiexplorer-in-asp-net-core-6

builder.Services
    .AddVersionedSwaggerGen()
    .AddApiVersioning(options =>
    {
        // options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        // options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("x-api-version"));

        options.Conventions.Add(new VersionByNamespaceConvention());
    })
    .AddCore()
    .AddFundaApi(
        builder.Configuration.GetSection("FundaHttpApiOptions").Bind,
        builder.Configuration.GetSection("RateLimitOptions").Bind)
    .AddLiteDb<RealEstateAgentsRetrievalStatus>(
        builder.Configuration.GetSection("LiteDbOptions").Bind)
    .AddLiteQueue<GetRealEstateAgent>(
        builder.Configuration.GetSection("LiteQueueOptions").Bind);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseVersionedSwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
