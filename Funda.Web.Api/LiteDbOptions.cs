﻿namespace Funda.Web.Api;

public class LiteDbOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string RetrievalStatusCollection { get; set; } = string.Empty;
    public string QueueCollection { get; set; } = string.Empty;
}
