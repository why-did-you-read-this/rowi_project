using rowi_project.Models.Dtos;

namespace rowi_project.Services
{
    public interface IAgentService
    {
        Task<int> CreateAgentAsync(CreateAgentDto dto);
        Task UpdateAgentAsync(UpdateAgentDto dto, CancellationToken cancellationToken);
        Task<AgentDto?> GetAgentByIdAsync(int id);
    }
}
