using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace Funda.Web.Api.Http;

public static class RetryPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> CircuitBreaker(int eventsBeforeBreaking, int secondsOfBreak) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(eventsBeforeBreaking, durationOfBreak: TimeSpan.FromSeconds(secondsOfBreak));

    public static IAsyncPolicy<HttpResponseMessage> ExponentialBackoff(
        int retryCount,
        Func<HttpResponseMessage, bool> retryNeeded,
        Action<TimeSpan, int>? beforeDelay = null) =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TaskCanceledException>()
            .OrResult(retryNeeded)
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: new ExponentialBackoffDurationProvider().Get,
                onRetry: (retryAttempt, delay, attempt, _) => beforeDelay?.Invoke(delay, attempt));

    private class ExponentialBackoffDurationProvider
    {
        private readonly Random _random = new();

        public TimeSpan Get(int retryAttempt)
        {
            var expBackoff = Math.Pow(2, retryAttempt);
            var maxJitter = (int)Math.Ceiling(expBackoff * 0.2);
            var finalBackoff = expBackoff + _random.Next(maxJitter);
            return TimeSpan.FromSeconds(finalBackoff);
        }
    }
}

