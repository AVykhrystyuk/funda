using System.ComponentModel.DataAnnotations;
using Funda.Common.CQRS.Abstractions;
using Funda.Core.Commands;
using Funda.Core.Models;
using Funda.Core.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Funda.Web.Api.Controllers.v1;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public class TopRealEstateAgentsRetrievalsController : ControllerBase
{
    /// <summary>
    /// Request to retrieve top N number of real estate agent that have the most object listed for sale.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="query">specific search criteria</param>
    /// <response code="202">retrieval</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(RetrievalDto), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CreateRetrieval(
        [FromBody] GetTopRealEstateAgentsQueryDto query,
        [FromServices] ICommandDispatcher dispatcher,
        CancellationToken cancellation)
    {
        var newRetrievalId = Guid.NewGuid();
        var command = query.ToRetrieveAgentsCommand(newRetrievalId);
        await dispatcher.Dispatch(command, cancellation);
        return Accepted(new RetrievalDto(newRetrievalId));
    }

    /// <summary>
    /// Get the real estate agent retrieval by retrieval id.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="retrievalId">retrieval Id</param>
    /// <response code="200">Retrieval</response>
    /// <response code="404">No retrieval for this retrievalId</response>
    [HttpGet("{retrievalId}")]
    [ProducesResponseType(typeof(RealEstateAgentsRetrievalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRetrieval(
        Guid retrievalId,
        [FromServices] IQueryDispatcher dispatcher,
        CancellationToken cancellation)
    {
        var query = new GetRealEstateAgentsRetrievalQuery(retrievalId);
        var retrieval = await dispatcher.Dispatch(query, cancellation);
        if (retrieval is null)
            return NotFound();
        return Ok(RealEstateAgentsRetrievalDto.From(retrieval));
    }

    /// <summary>
    /// Get real estate agent retrievals without the fetched data (to safe traffic).
    /// </summary>
    /// <remarks></remarks>
    /// <response code="200">Retrievals</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(RealEstateAgentsRetrievalDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRetrievals(
        [FromServices] IQueryDispatcher dispatcher,
        CancellationToken cancellation)
    {
        // TODO: Do we need to add a pagination here?

        var query = new GetRealEstateAgentsRetrievalsQuery();
        var retrievals = await dispatcher.Dispatch(query, cancellation);

        foreach (var retrieval in retrievals.Where(s => s.RealEstateAgents is not null))
            retrieval.RealEstateAgents = new RealEstateAgent[0];

        var dtos = retrievals.Select(RealEstateAgentsRetrievalDto.From).ToArray();
        return Ok(dtos);
    }
}

public record RetrievalDto(Guid RetrievalId);

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

public record RealEstateAgentsRetrievalDto(
    Guid RetrievalId,
    ProgressInfoDto? Progress,
    RealEstateAgentDto[]? Agents,
    string? ErrorMessage)
{
    public RetrievalStatus Status
    {
        get
        {
            if (ErrorMessage is not null)
                return RetrievalStatus.Error;
            if (Agents is not null)
                return RetrievalStatus.Completed;
            if (Progress is not null)
                return RetrievalStatus.Progress;
            return RetrievalStatus.None;
        }
    }

    public static RealEstateAgentsRetrievalDto From(RealEstateAgentsRetrieval retrieval) =>
        new(retrieval.RetrievalId,
            ProgressInfoDto.From(retrieval.Progress),
            retrieval.RealEstateAgents?.Select(RealEstateAgentDto.From).ToArray(),
            retrieval.ErrorMessage);
}

public enum RetrievalStatus { None, Progress, Completed, Error }
