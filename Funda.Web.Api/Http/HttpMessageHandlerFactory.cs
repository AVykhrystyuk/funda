using ComposableAsync;
using RateLimiter;

namespace Funda.Web.Api.Http;

public static class HttpMessageHandlerFactory
{
    public static HttpMessageHandler RateLimiter(RateLimitOptions options) =>
        TimeLimiter
            .GetFromMaxCountByInterval(options.MaxRequestCount, options.During)
            .AsDelegatingHandler();
}
