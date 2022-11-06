using FundaWebApiGeneratedClient;

internal class Application
{
    private readonly FundaWebApi _api;

    public Application(FundaWebApi api) =>
        _api = api ?? throw new ArgumentNullException(nameof(api));


    public TimeSpan ApiPollDelay = TimeSpan.FromSeconds(1);

    public async Task Run()
    {
        var queries = new[]
        {
            new GetTopRealEstateAgentsQueryDto
            {
                Location = "Amsterdam",
                TopNumberOfAgents = 10,
            },
            new GetTopRealEstateAgentsQueryDto
            {
                Location = "Amsterdam",
                Outdoors = new[] { "Tuin" },
                TopNumberOfAgents = 15,
            },
        };

        foreach (var query in queries)
        {
            await FetchAndDisplay(query);
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    private async Task FetchAndDisplay(GetTopRealEstateAgentsQueryDto query)
    {
        var fullLocation = GetFullLocation(query);
        Console.WriteLine($"{fullLocation} (top {query.TopNumberOfAgents}):");
        Console.WriteLine($"Starting to find out which (top {query.TopNumberOfAgents}) real estate agents in {fullLocation} have the most object listed for sale...");

        var retrievalCreated = await _api.CreateRetrievalAsync(query);

        RealEstateAgentsRetrievalDto? retrieval = null;
        ProgressInfoDto? prevProgress = null;
        var totalIsShown = false;
        while (retrieval?.Status is not RetrievalStatus.Completed or RetrievalStatus.Error)
        {
            retrieval = await _api.GetRetrievalAsync(retrievalCreated.RetrievalId);

            var progress = retrieval.Progress;
            var progressIsChanged = prevProgress is null || !Equals(progress, prevProgress);
            if (progress is not null && progressIsChanged)
            {
                if (!totalIsShown)
                {
                    Console.WriteLine($"Found {progress.Total} objects in total. Fetching them...");
                    totalIsShown = true;
                }
                else MoveCursorToLeft(); // to clear current console line

                var percent = progress.Fetched * 100 / progress.Total;
                Console.Write($"{progress.Total}/{progress.Fetched} ({percent}%)");
            }

            prevProgress = progress;
            await Task.Delay(ApiPollDelay);
        }

        Console.WriteLine();
        Console.WriteLine($"Top {query.TopNumberOfAgents} real estate agents in {fullLocation} that have the most object listed for sale:");

        var agents = retrieval?.Agents ?? Array.Empty<RealEstateAgentDto>();
        if (agents.Any())
        {
            var number = 0;
            foreach (var agent in agents)
                Console.WriteLine($"#{++number}. '{agent.AgentName}'  ({agent.ObjectCount} objects).");
        }
        else Console.WriteLine($"No real estate agents is found for {fullLocation}");
    }

    private bool Equals(ProgressInfoDto x, ProgressInfoDto y) =>
        x.Total == y.Total && x.Fetched == y.Fetched;

    private static string GetFullLocation(GetTopRealEstateAgentsQueryDto query) =>
        query.Outdoors is null
            ? query.Location
            : $"{query.Location} ({string.Join(", ", query.Outdoors)})";

    private static void MoveCursorToLeft() => Console.SetCursorPosition(0, Console.CursorTop);
}
