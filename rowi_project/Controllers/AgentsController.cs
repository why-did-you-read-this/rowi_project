using Microsoft.AspNetCore.Mvc;
using rowi_project.Models.Dtos;
using rowi_project.Services;

namespace rowi_project.Controllers;

[ApiController]
[Route("agents")] // [Route("api/[controller]")]
public class AgentsController(IAgentService agentService) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<int>> CreateAgent([FromBody] CreateAgentDto dto)
    {
        try
        {
            var id = await agentService.CreateAgentAsync(dto);
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
        if (dto.Id != id)
            return BadRequest("ID в пути и в теле запроса не совпадают");

        await agentService.UpdateAgentAsync(dto, cancellationToken);
        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AgentDto>> GetAgent(int id)
    {
        var result = await agentService.GetAgentByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }
}

