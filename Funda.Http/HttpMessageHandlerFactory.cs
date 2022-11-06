using ComposableAsync;
using RateLimiter;

namespace Funda.Http;

public static class HttpMessageHandlerFactory
{
    public static HttpMessageHandler RateLimiter(RateLimitOptions options) =>
        TimeLimiter
            .GetFromMaxCountByInterval(options.MaxRequestCount, options.During)
            .AsDelegatingHandler();
}
