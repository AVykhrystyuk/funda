﻿namespace Funda.Common.Http;

public class RateLimitOptions
{
    public int MaxRequestCount { get; set; } = 100;
    public TimeSpan During { get; set; }
}
