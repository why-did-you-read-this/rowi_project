using MediatR;

namespace rowi_project.Requests.Agents.Commands;

public record DeleteAgentCommand(int Id) : IRequest<Unit>;

