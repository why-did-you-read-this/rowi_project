using MediatR;
using Microsoft.AspNetCore.Mvc;
using rowi_project.Models.Dtos;
using rowi_project.Requests.Agents.Commands;
using rowi_project.Requests.Agents.Queries;

namespace rowi_project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateAgent([FromBody] CreateAgentDto dto, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(new CreateAgentCommand(dto), cancellationToken);
        return Ok(id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAgent(int id, [FromBody] UpdateAgentDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateAgentCommand(id, dto), cancellationToken);
        return Ok();

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AgentDto>> GetAgent(int id, CancellationToken cancellationToken)
    {
        //var agent = await agentService.GetAgentByIdAsync(id, cancellationToken);
        var agent = await mediator.Send(new GetAgentByIdQuery(id), cancellationToken);
        return Ok(agent);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] AgentSearchDto dto, CancellationToken cancellationToken)
    {
        //var agents = await agentService.SearchAgentsAsync(dto, cancellationToken);
        var agents = await mediator.Send(new SearchAgentsQuery(dto), cancellationToken);
        return Ok(agents);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAgent(int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAgentCommand(id), cancellationToken);
        return NoContent();
    }
}

