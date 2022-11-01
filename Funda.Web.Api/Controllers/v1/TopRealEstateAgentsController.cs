using System.ComponentModel.DataAnnotations;
using Funda.Common.CQRS.Abstractions;
using Funda.Core;
using Funda.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Funda.Web.Api.Controllers.v1;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public class TopRealEstateAgentsController : ControllerBase
{
    /// <summary>
    /// Retrieves top N number of real estate agent that have the most object listed for sale
    /// </summary>
    /// <remarks></remarks>
    /// <param name="query">specific search criteria</param>
    /// <response code="200">real estate agents</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(RealEstateAgentDto[]), 200)]
    public async Task<IActionResult> Get(
        [FromQuery] GetTopRealEstateAgentsQueryDto query,
        [FromServices] IQueryDispatcher dispatcher,
        CancellationToken cancellation)
    {
        var realEstateAgents = await dispatcher.Dispatch(query.ToQuery(), cancellation);
        return Ok(realEstateAgents.Select(RealEstateAgentDto.From).ToArray());
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
    public GetTopRealEstateAgentsQuery ToQuery() =>
        new(Location, Outdoor, TopNumberOfAgents);
}
