using rowi_project.Models.Dtos;

namespace rowi_project.Services;

public interface IAgentService
{
    Task<int> CreateAgentAsync(CreateAgentDto dto, CancellationToken cancellationToken);
    Task UpdateAgentAsync(int id, UpdateAgentDto dto, CancellationToken cancellationToken);
    Task<AgentDto?> GetAgentByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<AgentDto>> SearchAgentsAsync(AgentSearchDto searchDto, CancellationToken cancellationToken);
    Task DeleteAgentAsync(int id, CancellationToken cancellationToken);
}
