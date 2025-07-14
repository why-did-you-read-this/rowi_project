using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using rowi_project.Data;
using rowi_project.Exceptions;
using rowi_project.Models.Dtos;
using rowi_project.Models.Entities;
using rowi_project.Models;

namespace rowi_project.Services;

public class AgentService(AppDbContext context, IMapper mapper) : IAgentService
{
    public async Task<int> CreateAgentAsync(CreateAgentDto dto, CancellationToken cancellationToken)
    {
        if (dto.BankIds == null || dto.BankIds.Count == 0)
            throw new ArgumentException("Агент должен быть привязан хотя бы к одному банку.");

        var actualBankIds = await context.Banks
            .Where(b => dto.BankIds.Contains(b.Id))
            .Select(b => b.Id)
            .ToListAsync(cancellationToken);

        var missingBankIds = dto.BankIds.Except(actualBankIds).ToList();
        if (missingBankIds.Count > 0)
        {
            var idsString = string.Join(", ", missingBankIds);
            throw new ArgumentException($"Не найдены банки с этими id: {idsString}");
        }

        var agent = mapper.Map<Agent>(dto);

        agent.Banks = await context.Banks
            .Where(b => dto.BankIds.Contains(b.Id))
            .ToListAsync(cancellationToken);

        try
        {
            context.Agents.Add(agent);
            await context.SaveChangesAsync(cancellationToken);
            return agent.Id;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505") // Нарушение уникальности
        {
            return HandleUniqueViolation(pgEx);
        }
    }

    public async Task UpdateAgentAsync(int id, UpdateAgentDto dto, CancellationToken cancellationToken)
    {
        var agent = await context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
            ?? throw new ArgumentException("Агент не найден");

        if (dto.BankIds == null || dto.BankIds.Count == 0)
            throw new ArgumentException("Агент должен быть привязан хотя бы к одному банку.");

        var actualBankIds = await context.Banks
            .Where(b => dto.BankIds.Contains(b.Id))
            .Select(b => b.Id)
            .ToListAsync(cancellationToken);

        var missingBankIds = dto.BankIds.Except(actualBankIds).ToList();
        if (missingBankIds.Count > 0)
            throw new ArgumentException($"Не найдены банки с этими id: {string.Join(", ", missingBankIds)}");

        mapper.Map(dto, agent.Company);
        agent.Banks = await context.Banks.Where(b => dto.BankIds.Contains(b.Id)).ToListAsync(cancellationToken);
        agent.Important = dto.Important!.Value;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AgentDto?> GetAgentByIdAsync(int id, CancellationToken cancellationToken)
    {
        var agent = await context.Agents
        .Include(a => a.Company)
        .Include(a => a.Banks).ThenInclude(b => b.Company)
        .Where(a => a.Id == id)
        .ProjectTo<AgentDto>(mapper.ConfigurationProvider)
        .FirstOrDefaultAsync(cancellationToken);

        return agent ?? throw new NotFoundException($"Агент с ID {id} не найден");
    }

    public async Task<PagedResult<AgentDto>> SearchAgentsAsync(AgentSearchDto searchDto, CancellationToken cancellationToken)
    {
        var query = context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks)
                .ThenInclude(b => b.Company)
            .AsQueryable();

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
            "id" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Company.Id),
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

    public async Task DeleteAgentAsync(int id, CancellationToken cancellationToken)
    {
        var agent = await context.Agents
            .Include(a => a.Company)
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Агент не найден");

        agent.Company.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
    }

    private static int HandleUniqueViolation(PostgresException pgEx)
    {
        var message = pgEx.ConstraintName switch
        {
            "IX_Companies_ShortName" => "Компания с таким кратким наименованием уже существует",
            "IX_Companies_FullName" => "Компания с таким полным наименованием уже существует",
            "IX_Companies_Inn" => "Компания с таким ИНН уже существует",
            "IX_Companies_Ogrn" => "Компания с таким ОГРН уже существует",
            "IX_Companies_RepEmail" => "Представитель с таким email уже существует",
            "IX_Companies_RepPhoneNumber" => "Представитель с таким телефоном уже существует",
            _ => "Нарушено уникальное ограничение",
        };
        throw new DuplicateEntryException(message);
    }
}
