using MediatR;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Agents.Commands;

public record CreateAgentCommand(CreateAgentDto Dto) : IRequest<int>
{
    public CreateAgentDto Dto { get; set; } = Dto;
}
