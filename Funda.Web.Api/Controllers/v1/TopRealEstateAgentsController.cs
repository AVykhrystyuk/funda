using System.ComponentModel.DataAnnotations;
using Funda.Common.CQRS.Abstractions;
using Funda.Core;
using Funda.Core.Commands;
using Funda.Core.Models;
using Funda.Core.Queries;
using Funda.DocumentStore.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Funda.Web.Api.Controllers.v1;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public class TopRealEstateAgentsController : ControllerBase
{
    /// <summary>
    /// Request to retrieve top N number of real estate agent that have the most object listed for sale.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="query">specific search criteria</param>
    /// <response code="202">retrieval id</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateRetrieval(
        [FromBody] GetTopRealEstateAgentsQueryDto query,
        [FromServices] ICommandDispatcher dispatcher,
        CancellationToken cancellation)
    {
        var newRetrievalId = Guid.NewGuid();
        var command = query.ToRetrieveAgentsCommand(newRetrievalId);
        await dispatcher.Dispatch(command, cancellation);
        return Accepted(newRetrievalId);
    }

    /// <summary>
    /// Get the status for real estate agent retrieal by retrieval id.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="retrievalId">retrieval Id</param>
    /// <response code="200">Retrieval status</response>
    /// <response code="404">No retrieval for this retrievalId</response>
    [HttpGet("{retrievalId}/Status")]
    [ProducesResponseType(typeof(RealEstateAgentsRetrievalStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatus(
        Guid retrievalId,
        [FromServices] IQueryDispatcher dispatcher,
        CancellationToken cancellation)
    {
        var query = new GetRealEstateAgentsRetrievalStatusQuery(retrievalId);
        var retrieval = await dispatcher.Dispatch(query, cancellation);
        if (retrieval is null)
            return NotFound();
        return Ok(RealEstateAgentsRetrievalStatusDto.From(retrieval));
    }

    /// <summary>
    /// Get lightweight status for real estate agent retrievals without the fetched data (to safe traffic).
    /// </summary>
    /// <remarks></remarks>
    /// <response code="200">retrieal statuses</response>
    [HttpGet("Statuses")]
    [ProducesResponseType(typeof(RealEstateAgentsRetrievalStatusDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatuses(
        [FromServices] IQueryDispatcher dispatcher,
        CancellationToken cancellation)
    {
        // TODO: Do we need to add a pagination here?

        var query = new GetRealEstateAgentsRetrievalStatusesQuery();
        var retrievals = await dispatcher.Dispatch(query, cancellation);

        foreach (var retrieval in retrievals.Where(s => s.RealEstateAgents is not null))
            retrieval.RealEstateAgents = new RealEstateAgent[0];

        var dtos = retrievals.Select(RealEstateAgentsRetrievalStatusDto.From).ToArray();
        return Ok(dtos);
    }
}

public record GetTopRealEstateAgentsQueryDto(
    string Location,
    string? Outdoor = null,
    [Range(1, 1000)] int TopNumberOfAgents = 10)
{
    public RetrieveRealEstateAgentsCommand ToRetrieveAgentsCommand(Guid retrievalId) => 
        new(retrievalId, Location, Outdoor, TopNumberOfAgents);
}

public record ProgressInfoDto(long Total, long Fetched)
{
    public static ProgressInfoDto? From(ProgressInfo? info) =>
        info is null ? null : new(info.Total, info.Fetched);
}

public record RealEstateAgentDto(long AgentId, string AgentName, int ObjectCount)
{
    public static RealEstateAgentDto From(RealEstateAgent agent) =>
      new(agent.AgentId, agent.AgentName, agent.ObjectCount);
}

public record RealEstateAgentsRetrievalStatusDto(
    ProgressInfoDto? Progress,
    RealEstateAgentDto[]? Agents,
    string? ErrorMessage)
{
    public RetrievalStatusType Type
    {
        get
        {
            if (ErrorMessage is not null)
                return RetrievalStatusType.Error;
            if (Agents is not null)
                return RetrievalStatusType.Completed;
            if (Progress is not null)
                return RetrievalStatusType.Progress;
            return RetrievalStatusType.None;
        }
    }

    public static RealEstateAgentsRetrievalStatusDto From(RealEstateAgentsRetrievalStatus status) =>
        new(ProgressInfoDto.From(status.Progress),
            status.RealEstateAgents?.Select(RealEstateAgentDto.From).ToArray(),
            status.ErrorMessage);
}

public enum RetrievalStatusType { None, Progress, Completed, Error }
