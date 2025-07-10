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
        var id = await agentService.CreateAgentAsync(dto, cancellationToken);
        return Ok(new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAgent(int id, [FromBody] UpdateAgentDto dto, CancellationToken cancellationToken)
    {
        await agentService.UpdateAgentAsync(id, dto, cancellationToken);
        return Ok();

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AgentDto>> GetAgent(int id, CancellationToken cancellationToken)
    {
        var agent = await agentService.GetAgentByIdAsync(id, cancellationToken);
        return Ok(agent);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] AgentSearchDto dto, CancellationToken cancellationToken)
    {
        var agents = await agentService.SearchAgentsAsync(dto, cancellationToken);
        return Ok(agents);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAgent(int id, CancellationToken cancellationToken)
    {
        await agentService.DeleteAgentAsync(id, cancellationToken);
        return NoContent();
    }
}

