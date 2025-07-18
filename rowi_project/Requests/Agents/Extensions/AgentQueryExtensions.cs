using Microsoft.EntityFrameworkCore;
using rowi_project.Models.Dtos;
using rowi_project.Models.Entities;

namespace rowi_project.Requests.Agents.Extensions;

public static class AgentQueryExtensions
{
    public static IQueryable<Agent> ApplyFilters(this IQueryable<Agent> query, AgentSearchDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Inn))
            query = query.Where(a => EF.Functions.ILike(a.Company.Inn, $"%{dto.Inn}%"));

        if (!string.IsNullOrEmpty(dto.Ogrn))
            query = query.Where(a => EF.Functions.ILike(a.Company.Ogrn, $"%{dto.Ogrn}%"));

        if (!string.IsNullOrEmpty(dto.RepPhoneNumber))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepPhoneNumber, $"%{dto.RepPhoneNumber}%"));

        if (!string.IsNullOrEmpty(dto.RepEmail))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepEmail, $"%{dto.RepEmail}%"));

        if (!string.IsNullOrEmpty(dto.ShortName))
            query = query.Where(a => EF.Functions.ILike(a.Company.ShortName, $"%{dto.ShortName}%"));

        if (!string.IsNullOrEmpty(dto.RepName))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepName, $"%{dto.RepName}%"));

        if (!string.IsNullOrEmpty(dto.RepSurName))
            query = query.Where(a => EF.Functions.ILike(a.Company.RepSurName, $"%{dto.RepSurName}%"));

        if (dto.Important is not null)
            query = query.Where(a => a.Important == dto.Important);

        if (dto.OgrnDateFrom is not null)
            query = query.Where(a => a.Company.OgrnDateOfAssignment >= dto.OgrnDateFrom);

        if (dto.OgrnDateTo is not null)
            query = query.Where(a => a.Company.OgrnDateOfAssignment <= dto.OgrnDateTo);

        return query;
    }

    public static IQueryable<Agent> ApplySorting(this IQueryable<Agent> query, AgentSearchDto dto)
    {
        return dto.SortBy?.ToLower() switch
        {
            "shortname" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Company.ShortName)
                : query.OrderBy(a => a.Company.ShortName),

            "inn" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Company.Inn)
                : query.OrderBy(a => a.Company.Inn),

            "ogrn" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Company.Ogrn)
                : query.OrderBy(a => a.Company.Ogrn),

            "repname" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Company.RepName)
                : query.OrderBy(a => a.Company.RepName),

            "repsurname" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Company.RepSurName)
                : query.OrderBy(a => a.Company.RepSurName),

            "ogrndateofassignment" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Company.OgrnDateOfAssignment)
                : query.OrderBy(a => a.Company.OgrnDateOfAssignment),

            "important" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Important).ThenBy(a => a.Id)
                : query.OrderBy(a => a.Important).ThenBy(a => a.Id),

            "id" => dto.SortDirection == "desc"
                ? query.OrderByDescending(a => a.Id)
                : query.OrderBy(a => a.Id),

            _ => query.OrderBy(a => a.Id)
        };
    }

    public static IQueryable<Agent> ApplyPaging(this IQueryable<Agent> query, AgentSearchDto dto)
    {
        var skip = (dto.PageNumber - 1) * dto.PageSize;
        return query.Skip(skip).Take(dto.PageSize);
    }
}
