using MediatR;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Agents.Commands;

public record UpdateAgentCommand(int Id, UpdateAgentDto Dto) : IRequest<Unit>;
