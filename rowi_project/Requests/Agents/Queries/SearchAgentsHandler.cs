using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Models;
using rowi_project.Models.Dtos;
using rowi_project.Requests.Agents.Extensions;

namespace rowi_project.Requests.Agents.Queries;

public class SearchAgentsHandler(AppDbContext context, IMapper mapper) : IRequestHandler<SearchAgentsQuery, PagedResult<AgentDto>>
{
    public async Task<PagedResult<AgentDto>> Handle(SearchAgentsQuery request, CancellationToken cancellationToken)
    {
        var searchDto = request.SearchDto;

        var query = context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks).ThenInclude(b => b.Company)
            .AsQueryable()
            .AsSplitQuery()
            .ApplyFilters(searchDto)
            .ApplySorting(searchDto)
            .ApplyPaging(searchDto);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .ProjectTo<AgentDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<AgentDto>
        {
            TotalCount = totalCount,
            Items = items
        };
    }
}

