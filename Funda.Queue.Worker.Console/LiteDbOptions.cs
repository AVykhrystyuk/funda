namespace Funda.Queue.Worker.Console;

public class LiteDbOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string RetrievalStatusCollection { get; set; } = string.Empty;
    public string QueueCollection { get; set; } = string.Empty;
}
