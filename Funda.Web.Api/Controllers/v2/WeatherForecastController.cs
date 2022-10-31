using Microsoft.AspNetCore.Mvc;

namespace Funda.Web.Api.Controllers.v2;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
public class WeatherForecastController : ControllerBase
{
    // <param name="id" example="123">The product id</param>

    /// <summary>
    /// Retrieves a specific product by unique id
    /// </summary>
    /// <remarks>Awesomeness!</remarks>
    /// <response code="200">Product retrieved</response>
    /// <response code="404">Product not found</response>
    /// <response code="500">Oops! Can't lookup your product right now</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Response), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    public IActionResult Get(long id, [FromServices] ILogger<WeatherForecastController> logger) => Ok(new Response("v2"));
}

public record Response(string Text);
