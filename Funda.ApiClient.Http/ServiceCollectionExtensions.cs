﻿using Funda.ApiClient.Abstractions;
using Funda.Common.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Funda.ApiClient.Http;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFundaApi(this IServiceCollection services,
        Action<FundaHttpApiOptions> configureApiOptions,
        Action<RateLimitOptions> configureRateLimitOptions)
    {
        services.AddOptions<FundaHttpApiOptions>()
            .Configure(configureApiOptions)
            .Validate(o => !string.IsNullOrEmpty(o.ApiKey), $"{nameof(FundaHttpApiOptions.ApiKey)} must be provided.")
            .Validate(o => !string.IsNullOrEmpty(o.FeedsBaseUrl), $"{nameof(FundaHttpApiOptions.FeedsBaseUrl)} must be provided.");

        var rateLimitOptions = new RateLimitOptions(); // services.AddOptions is not needed here
        configureRateLimitOptions(rateLimitOptions);

        services.AddHttpClient<IFundaApiClient, FundaHttpApiClient>()
            .ConfigureHttpClient((p, client) =>
            {
                var options = p.GetRequiredService<IOptions<FundaHttpApiOptions>>().Value;
                client.Timeout = options.Timeout;
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(6))
            .ConfigurePrimaryHttpMessageHandler(() =>
                HttpMessageHandlerFactory.RateLimiter(rateLimitOptions))
            .AddPolicyHandler((services, request) =>
                RetryPolicies.ExponentialBackoff(
                    retryCount: 5,
                    retryNeeded: FundaHttpApiClient.IsTooManyRequests,
                    beforeDelay: LogBeforeDelay(services)))
            .AddPolicyHandler(
                RetryPolicies.CircuitBreaker(eventsBeforeBreaking: 12, secondsOfBreak: 30));

        return services;

        static Action<TimeSpan, int> LogBeforeDelay(IServiceProvider services)
        {
            var logger = services.GetService<ILogger<FundaHttpApiClient>>();
            return (delay, attempt) =>
                logger?.LogTrace("Retry {attempt}: Delaying for {delay} seconds", attempt, delay.TotalSeconds);
        }
    }
}
