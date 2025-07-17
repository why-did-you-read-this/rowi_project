using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Models;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Agents.Queries;

public class SearchAgentsHandler(AppDbContext context, IMapper mapper) : IRequestHandler<SearchAgentsQuery, PagedResult<AgentDto>>
{
    public async Task<PagedResult<AgentDto>> Handle(SearchAgentsQuery request, CancellationToken cancellationToken)
    {
        var searchDto = request.SearchDto;

        var query = context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks).ThenInclude(b => b.Company)
            .AsQueryable().AsSplitQuery();

        // Фильтрация
        if (!string.IsNullOrEmpty(searchDto.Inn))
            query = query.Where(a => EF.Functions.ILike(a.Company.Inn, $"%{searchDto.Inn}%"));

        if (!string.IsNullOrEmpty(searchDto.Ogrn))
            query = query.Where(a => EF.Functions.ILike(a.Company.Ogrn, $"%{searchDto.Ogrn}%"));

        if (!string.IsNullOrEmpty(searchDto.RepPhoneNumber))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepPhoneNumber, $"%{searchDto.RepPhoneNumber}%"));

        if (!string.IsNullOrEmpty(searchDto.RepEmail))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepEmail, $"%{searchDto.RepEmail}%"));

        if (!string.IsNullOrEmpty(searchDto.ShortName))
            query = query.Where(a => EF.Functions.ILike(a.Company.ShortName, $"%{searchDto.ShortName}%"));

        if (!string.IsNullOrEmpty(searchDto.RepName))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepName, $"%{searchDto.RepName}%"));

        if (!string.IsNullOrEmpty(searchDto.RepSurName))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepSurName, $"%{searchDto.RepSurName}%"));

        if (searchDto.Important is not null)
            query = query.Where(a => a.Important == searchDto.Important);

        if (searchDto.OgrnDateFrom is not null)
            query = query.Where(a => a.Company.OgrnDateOfAssignment >= searchDto.OgrnDateFrom);

        if (searchDto.OgrnDateTo is not null)
            query = query.Where(a => a.Company.OgrnDateOfAssignment <= searchDto.OgrnDateTo);

        var totalCount = await query.CountAsync(cancellationToken);

        // Сортировка
        query = searchDto.SortBy?.ToLower() switch
        {
            "shortname" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.ShortName) : query.OrderBy(a => a.Company.ShortName),
            "inn" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.Inn) : query.OrderBy(a => a.Company.Inn),
            "ogrn" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.Ogrn) : query.OrderBy(a => a.Company.Ogrn),
            "repname" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.RepName) : query.OrderBy(a => a.Company.RepName),
            "repsurname" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.RepSurName) : query.OrderBy(a => a.Company.RepSurName),
            "ogrndateofassignment" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.OgrnDateOfAssignment) : query.OrderBy(a => a.Company.OgrnDateOfAssignment),
            "important" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Important).ThenBy(a => a.Id) : query.OrderBy(a => a.Important).ThenBy(a => a.Id),
            "id" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id),
            _ => query.OrderBy(a => a.Id)
        };

        // Пагинация
        int skip = (searchDto.PageNumber - 1) * searchDto.PageSize;
        query = query.Skip(skip).Take(searchDto.PageSize);

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

