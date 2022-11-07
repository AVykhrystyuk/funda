# Funda

Find out which real estate agent in the Netherlands has the most properties for sale at [Funda](https://www.funda.nl/).
Get a list of top N agents for specific search criteria: 
- location (Amsterdam, Rotterdam, etc) 
- and outdoor space type (Balcony, Roof, Garden, etc).


## Getting Started
ASP.NET Core 6, .NET 6, C# 

Solution is built using Clean Architecture + CQRS and asynchronous processing of a long-running request (like fetching a large set of objects listed in Funda).


### Key things:
- Business logic is located in `Funda.Core`.
- Tired to use generated WCF Service Reference (http://partnerapi.funda.nl/feeds/Aanbod.svc?wsdl) but it keept failing with non-self-explanatory errors so I moved to calling the service from http client.
- Retries and circuit breaker policies are implemented in `Funda.ApiClient.Http` (ServiceCollectionExtensions). Also there is `HttpMessageHandlerFactory.RateLimiter` to handle the Funda's "rate limiter" - API requests are limited to 100 requests per minute.
- Apply `SortBy.DateAscending` for Funda search queries to avoid a `"Page drift"` when new objects are added during the fetch.
- To support idempotent `POST` (CreateRetrieval) we have to delegate the creation of unique search identifiers to trusted clients.

## System design decisions

We don't know the size of the dataset to be processed, Funda can store millions of objects listed for sale. 
So processing all of them synchronously (within a single long-running API request/response call) does not make any sense if we want to have a scalable solution. 

#### This is what the communication between the client and WebApi looks like:
```mermaid
%% This diagram needs to be rendered, for example, GitHub renders it by default.
sequenceDiagram
    Client->>+Funda.Web.Api: [POST] /TopRealEstateAgentsRetrievals { "newRetrievalId": "...", "location": "..." }
    Funda.Web.Api-->>-Client: [202] { "retrievalId": "..." }
    Note over Client,Funda.Web.Api: Request for agents' retrieval is accepted
    
    Client->>+Funda.Web.Api: [GET] /TopRealEstateAgentsRetrievals/{retrievalId}
    Funda.Web.Api-->>-Client: [200] { "status": "Enqueued" }
    Note over Client,Funda.Web.Api: The retrieval is not launched yet
        
    Client->>+Funda.Web.Api: [GET] /TopRealEstateAgentsRetrievals/{retrievalId}
    Funda.Web.Api-->>-Client: [200] { "status": "InProgress", "progress": { "Total": 120, "Fetched": 10 } }
    Note over Client,Funda.Web.Api: The retrieval is launched
    
    Client->>+Funda.Web.Api: [GET] /TopRealEstateAgentsRetrievals/{retrievalId}
    Funda.Web.Api-->>-Client: [200] { "status": "Completed", "progress": { "Total": 120, "Fetched": 120 }, "agents": [...] }
    Note over Client,Funda.Web.Api: The retrieval is completed, and "agents" array is populated in the response
```

#### This is how the search request is handled under the hood:
```mermaid
%% This diagram needs to be rendered, for example, GitHub renders it by default.
flowchart TB
    C([Client])
    WebApi(Funda.Web.Api)
    Queue[[Funda.Queue.LiteQueue]]
    Worker(Funda.Queue.Worker.Console)
    KVStore[(Funda.DocumentStore.LiteDb)]
    ApiClient[[Funda.ApiClient.Http]]
    
    C-- POST retrieval query -->WebApi-- pushes message to queue --> Queue
    WebApi-- returns retrieval id back -->C
    Worker<-- waits for a message  -->Queue
    Worker<-- fetches the data  --> ApiClient
    Worker-- stores the fetch progress + the fetched data --> KVStore
```


## Run the solution with WebApi Consumer (recommended)
* Restore NuGet packages.
* Make all 3 projects `Funda.Web.Api`, `Funda.Web.Api.Consumer.Console` & `Funda.Queue.Worker.Console` to be Multiple Startup Projects (with Action `'Start'`) in Visual Studio (VS) so that when you run the solution all of them start at the same time.
* Run the solution from VS.
* Look into `Funda.Web.Api.Consumer.Console`.


## Run the solution without WebApi Consumer
* Restore NuGet packages.
* Make the following 2 projects `Funda.Web.Api` & `Funda.Queue.Worker.Console` to be Multiple Startup Projects (with Action `'Start'`) in Visual Studio (VS) so that when you run the solution all of them start at the same time.
* Run the solution from VS.
* Run the following command to issue a fetch request (or use a Swagger UI) that will be enqueued and processed asynchronously by a background worker. The Api will return you `retrievalId` (Guid) that is required to receive the result of the background fetch:
```bash
curl -X 'POST' \
  'https://localhost:7215/v1/TopRealEstateAgentsRetrievals' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
    "newRetrievalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "location": "Amsterdam",
    "outdoors": ["Tuin"],
    "topNumberOfAgents": 10
  }'
```
* Poll the following command to see the fetch process:
```bash
curl -X 'GET' \
  'https://localhost:7215/v1/TopRealEstateAgentsRetrievals/3fa85f64-5717-4562-b3fc-2c963f66afa6' \
  -H 'accept: text/plain'
```
