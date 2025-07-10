using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using rowi_project.Data;
using rowi_project.Models.Dtos;
using rowi_project.Models.Entities;

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
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при сохранении агента: {ex.Message}");
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

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505") // Нарушение уникальности
        {
            HandleUniqueViolation(pgEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при обновлении агента: {ex.Message}");
        }
    }

    public async Task<AgentDto?> GetAgentByIdAsync(int id)
    {
        return await context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks).ThenInclude(b => b.Company)
            .Where(a => a.Id == id)
            .ProjectTo<AgentDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<AgentDto>> SearchAgentsAsync(AgentSearchDto searchDto, CancellationToken cancellationToken)
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

        if (searchDto.OgrnDateFrom is not null)
            query = query.Where(a => a.Company.OgrnDateOfAssignment >= searchDto.OgrnDateFrom);

        if (searchDto.OgrnDateTo is not null)
            query = query.Where(a => a.Company.OgrnDateOfAssignment <= searchDto.OgrnDateTo);

        // Сортировка
        query = searchDto.SortBy?.ToLower() switch
        {
            "shortName" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.ShortName) : query.OrderBy(a => a.Company.ShortName),
            "inn" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.Inn) : query.OrderBy(a => a.Company.Inn),
            "ogrn" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.Ogrn) : query.OrderBy(a => a.Company.Ogrn),
            "repname" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.RepName) : query.OrderBy(a => a.Company.RepName),
            "repsurname" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.RepSurName) : query.OrderBy(a => a.Company.RepSurName),
            "ogrndateofassignment" => searchDto.SortDirection == "desc" ? query.OrderByDescending(a => a.Company.OgrnDateOfAssignment) : query.OrderBy(a => a.Company.OgrnDateOfAssignment),
            _ => query.OrderBy(a => a.Id)
        };

        // Пагинация
        int skip = (searchDto.PageNumber - 1) * searchDto.PageSize;
        query = query.Skip(skip).Take(searchDto.PageSize);

        return await query.ProjectTo<AgentDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
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
        throw pgEx.ConstraintName switch
        {
            "IX_Companies_ShortName" => new ArgumentException("Компания с таким кратким наименованием уже существует"),
            "IX_Companies_FullName" => new ArgumentException("Компания с таким полным наименованием уже существует"),
            "IX_Companies_Inn" => new ArgumentException("Компания с таким ИНН уже существует"),
            "IX_Companies_Ogrn" => new ArgumentException("Компания с таким ОГРН уже существует"),
            "IX_Companies_RepEmail" => new ArgumentException("Представитель с таким email уже существует"),
            "IX_Companies_RepPhoneNumber" => new ArgumentException("Представитель с таким телефоном уже существует"),
            _ => new ArgumentException("Нарушено уникальное ограничение"),
        };
    }
}
