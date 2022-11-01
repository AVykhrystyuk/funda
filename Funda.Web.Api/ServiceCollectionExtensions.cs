using Funda.ApiClient.Abstractions;
using Funda.ApiClient.Http;
using Funda.Web.Api.Http;

namespace Funda.Web.Api;

public static class ServiceCollectionExtensions
{
    public static void AddFundaApi(this IServiceCollection services, 
        Action<FundaHttpApiOptions> configureApiOptions, 
        Action<RateLimitOptions> configureRateLimitOptions)
    {
        services.AddOptions<FundaHttpApiOptions>()
            .Configure(configureApiOptions)
            .Validate(o => !string.IsNullOrEmpty(o.ApiKey), $"{nameof(FundaHttpApiOptions.ApiKey)} must be provided.")
            .Validate(o => !string.IsNullOrEmpty(o.FeedsBaseUrl), $"{nameof(FundaHttpApiOptions.FeedsBaseUrl)} must be provided.");

        var rateLimitOptions = new RateLimitOptions();
        configureRateLimitOptions(rateLimitOptions);

        services.AddHttpClient<IFundaApiClient, FundaHttpApiClient>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(6))
            .ConfigurePrimaryHttpMessageHandler(() => HttpMessageHandlerFactory.RateLimiter(rateLimitOptions))
            .AddPolicyHandler((services, request) =>
                RetryPolicies.ExponentialBackoff(
                    retryCount: 3, 
                    retryNeeded: FundaHttpApiClient.IsTooManyRequests,
                    beforeDelay: LogBeforeDelay(services)))
            .AddPolicyHandler(
                RetryPolicies.CircuitBreaker(eventsBeforeBreaking: 12, secondsOfBreak: 30));

        static Action<TimeSpan, int> LogBeforeDelay(IServiceProvider services)
        {
            var logger = services.GetService<ILogger<FundaHttpApiClient>>();
            return (delay, attempt) =>
                logger?.LogTrace("Retry {attempt}: Delaying for {delay} seconds", attempt, delay.TotalSeconds);
        }
    }
}
