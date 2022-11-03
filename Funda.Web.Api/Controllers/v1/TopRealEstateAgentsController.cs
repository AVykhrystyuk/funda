using System.ComponentModel.DataAnnotations;
using Funda.Common.CQRS.Abstractions;
using Funda.Core;
using Funda.Core.Commands;
using Funda.Core.Models;
using Funda.Core.Queries;
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
    /// <response code="200">retrieval id</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(RealEstateAgentDto[]), 200)]
    public async Task<Guid> CreateRetrieval(
        [FromBody] GetTopRealEstateAgentsQueryDto query,
        [FromServices] ICommandDispatcher dispatcher,
        CancellationToken cancellation)
    {
        var newRetrievalId = Guid.NewGuid();
        var command = query.ToRetrieveAgentsCommand(newRetrievalId);
        await dispatcher.Dispatch(command, cancellation);
        return newRetrievalId;
    }

    /// <summary>
    /// Checks the status for real estate agent retrieal by retrieval id.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="retrievalId">retrieval Id</param>
    /// <response code="200">retrieal status</response>
    [HttpGet("{requestId}/Status")]
    [ProducesResponseType(typeof(RealEstateAgentDto[]), 200)]
    public async Task<IActionResult> GetStatus(
        Guid retrievalId,
        [FromServices] IQueryDispatcher dispatcher,
        CancellationToken cancellation)
    {
        //var realEstateObjects = await dispatcher.Dispatch(query.ToQuery(), cancellation);
        //var realEstateAgents = aggregator.GetTopAgents(realEstateObjects, query.TopNumberOfAgents);
        //return Ok(realEstateAgents.Select(RealEstateAgentDto.From).ToArray());
        return Ok(new RealEstateAgentDto[0]);
    }
}

public record RealEstateAgentDto(long AgentId, string AgentName, int ObjectCount)
{
    public static RealEstateAgentDto From(RealEstateAgent agent) =>
        new(agent.AgentId, agent.AgentName, agent.ObjectCount);
}

public record GetTopRealEstateAgentsQueryDto(
    string Location,
    string? Outdoor = null,
    [Range(1, 1000)] int TopNumberOfAgents = 10)
{
    public RetrieveRealEstateAgentsCommand ToRetrieveAgentsCommand(Guid retrievalId) => 
        new(retrievalId, Location, Outdoor, TopNumberOfAgents);
}
