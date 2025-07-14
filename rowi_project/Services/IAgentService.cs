using rowi_project.Models.Dtos;
using rowi_project.Models;

namespace rowi_project.Services;

public interface IAgentService
{
    Task<int> CreateAgentAsync(CreateAgentDto dto, CancellationToken cancellationToken);
    Task UpdateAgentAsync(int id, UpdateAgentDto dto, CancellationToken cancellationToken);
    Task<AgentDto?> GetAgentByIdAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<AgentDto>> SearchAgentsAsync(AgentSearchDto searchDto, CancellationToken cancellationToken);
    Task DeleteAgentAsync(int id, CancellationToken cancellationToken);
}
