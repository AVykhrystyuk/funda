using Funda.ApiClient.Http;
using Funda.Core;
using Funda.Core.Models;
using Funda.Core.QueueMessages;
using Funda.DocumentStore.LiteDb;
using Funda.Queue.LiteQueue;
using Funda.Queue.Worker.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var dequeueInterval = TimeSpan.FromSeconds(1);
var workerRunInterval = TimeSpan.FromSeconds(1);

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .AddCore()
            .AddFundaApi(
                context.Configuration.GetSection("FundaHttpApiOptions").Bind,
                context.Configuration.GetSection("RateLimitOptions").Bind)
            .AddLiteDb<RealEstateAgentsRetrievalStatus>(
                context.Configuration.GetSection("LiteDbOptions").Bind)
            .AddLiteQueue<GetRealEstateAgent>(
                context.Configuration.GetSection("LiteQueueOptions").Bind,
                dequeueInterval)
            .AddTransient<RealEstateAgentsWorker>();
    })
    .Build();

var worker = host.Services.GetRequiredService<RealEstateAgentsWorker>();
await worker.Run(workerRunInterval);
