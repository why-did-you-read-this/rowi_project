using Microsoft.AspNetCore.Mvc;
using rowi_project.Models.Dtos;
using rowi_project.Services;

namespace rowi_project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentsController(IAgentService agentService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateAgent([FromBody] CreateAgentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var id = await agentService.CreateAgentAsync(dto, cancellationToken);
            return Ok(new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAgent(int id, [FromBody] UpdateAgentDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await agentService.UpdateAgentAsync(id, dto, cancellationToken);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AgentDto>> GetAgent(int id)
    {
        var result = await agentService.GetAgentByIdAsync(id);
        if (result == null)
            return NotFound(new ProblemDetails
            {
                Title = "Agent not found",
                Detail = $"Agent with ID = {id} does not exist",
                Status = 404,
                Instance = HttpContext.Request.Path
            });

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] AgentSearchDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var agents = await agentService.SearchAgentsAsync(dto, cancellationToken);
            return Ok(agents);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAgent(int id, CancellationToken cancellationToken)
    {
        try
        {
            await agentService.DeleteAgentAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

