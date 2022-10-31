using Funda.Web.Api.Swagger;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer(); // 'minimal API' is not used https://stackoverflow.com/questions/71932980/what-is-addendpointsapiexplorer-in-asp-net-core-6
builder.Services.AddVersionedSwaggerGen();

builder.Services.AddApiVersioning(options =>
{
    // options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    // options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"));

    options.Conventions.Add(new VersionByNamespaceConvention());
});

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
