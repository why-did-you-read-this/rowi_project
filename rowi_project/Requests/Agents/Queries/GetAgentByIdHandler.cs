using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Exceptions;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Agents.Queries;

public class GetAgentByIdHandler(AppDbContext context, IMapper mapper) : IRequestHandler<GetAgentByIdQuery, AgentDto>
{
    public async Task<AgentDto> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
    {
        var agent = await context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks).ThenInclude(b => b.Company)
            .Where(a => a.Id == request.Id)
            .ProjectTo<AgentDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        return agent ?? throw new NotFoundException($"Агент с ID {request.Id} не найден");
    }
}

