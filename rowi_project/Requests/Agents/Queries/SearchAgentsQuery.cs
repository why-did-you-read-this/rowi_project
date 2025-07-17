using MediatR;
using rowi_project.Models;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Agents.Queries;

public record SearchAgentsQuery(AgentSearchDto SearchDto) : IRequest<PagedResult<AgentDto>>;

