using Funda.Common.Http;
using FundaWebApiGeneratedClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json");

var config = configuration.Build();


var services = new ServiceCollection();

services
    .AddHttpClient(name: nameof(FundaWebApi))
    .AddPolicyHandler((services, request) =>
        RetryPolicies.ExponentialBackoff(
            retryCount: 5,
            retryNeeded: r => false))
    .AddPolicyHandler(
        RetryPolicies.CircuitBreaker(eventsBeforeBreaking: 12, secondsOfBreak: 30));
services
    .AddSingleton(p => new FundaWebApi(
        baseUrl: config.GetSection("FundaWebApi:BaseUrl").Value,
        httpClient: p.GetRequiredService<IHttpClientFactory>().CreateClient(name: nameof(FundaWebApi))))
    .AddTransient<Application>();

var provider = services.BuildServiceProvider();

var app = provider.GetRequiredService<Application>();
await app.Run();
