using MediatR;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Agents.Queries;

public record GetAgentByIdQuery(int Id) : IRequest<AgentDto>;
