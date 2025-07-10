using Microsoft.EntityFrameworkCore;
using Npgsql;
using rowi_project.Data;
using rowi_project.Models.Dtos;
using rowi_project.Models.Entities;

namespace rowi_project.Services;

public class AgentService(AppDbContext context) : IAgentService
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

        var company = new Company
        {
            ShortName = dto.ShortName,
            FullName = dto.FullName,
            Inn = dto.Inn,
            Kpp = dto.Kpp,
            Ogrn = dto.Ogrn,
            OgrnDateOfAssignment = (DateOnly)dto.OgrnDateOfAssignment!,
            RepName = dto.RepName,
            RepSurName = dto.RepSurname,
            RepPatronymic = dto.RepPatronymic,
            RepEmail = dto.RepEmail,
            RepPhoneNumber = dto.RepPhoneNumber,
        };

        var agent = new Agent
        {
            Important = (bool)dto.Important!,
            Company = company,
            Banks = await context.Banks
                .Where(b => dto.BankIds.Contains(b.Id))
                .ToListAsync(cancellationToken)
        };

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
        {
            var idsString = string.Join(", ", missingBankIds);
            throw new ArgumentException($"Не найдены банки с этими id: {idsString}");
        }


        var company = agent.Company;
        company.ShortName = dto.ShortName;
        company.FullName = dto.FullName;
        company.Inn = dto.Inn;
        company.Kpp = dto.Kpp;
        company.Ogrn = dto.Ogrn;
        company.OgrnDateOfAssignment = (DateOnly)dto.OgrnDateOfAssignment!;
        company.RepName = dto.RepName;
        company.RepSurName = dto.RepSurname;
        company.RepPatronymic = dto.RepPatronymic;
        company.RepEmail = dto.RepEmail;
        company.RepPhoneNumber = dto.RepPhoneNumber;

        agent.Banks = await context.Banks
            .Where(b => dto.BankIds!.Contains(b.Id))
            .ToListAsync(cancellationToken);

        agent.Important = (bool)dto.Important!;

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
        var agent = await context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks)
                .ThenInclude(b => b.Company)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (agent == null) return null;

        return new AgentDto
        {
            Id = agent.Id,
            RepFullName = $"{agent.Company.RepSurName} {agent.Company.RepName} {agent.Company.RepPatronymic}",
            RepEmail = agent.Company.RepEmail,
            RepPhoneNumber = agent.Company.RepPhoneNumber,
            ShortName = agent.Company.ShortName,
            FullName = agent.Company.FullName,
            Inn = agent.Company.Inn,
            Kpp = agent.Company.Kpp,
            Ogrn = agent.Company.Ogrn,
            OgrnDateOfAssignment = agent.Company.OgrnDateOfAssignment,
            Important = agent.Important,
            Banks = [.. agent.Banks.Select(b => new BankDto
            {
                Id = b.Id,
                ShortName = b.Company.ShortName
            })]
        };
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
        Console.WriteLine($"searchDto.SortBy {searchDto.SortBy}");
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

        var agents = await query.ToListAsync(cancellationToken);

        return [.. agents.Select(agent => new AgentDto
        {
            Id = agent.Id,
            RepFullName = $"{agent.Company.RepSurName} {agent.Company.RepName} {agent.Company.RepPatronymic}",
            RepEmail = agent.Company.RepEmail,
            RepPhoneNumber = agent.Company.RepPhoneNumber,
            ShortName = agent.Company.ShortName,
            FullName = agent.Company.FullName,
            Inn = agent.Company.Inn,
            Kpp = agent.Company.Kpp,
            Ogrn = agent.Company.Ogrn,
            OgrnDateOfAssignment =  agent.Company.OgrnDateOfAssignment,
            Important = agent.Important,
            Banks = [.. agent.Banks.Select(b => new BankDto
            {
                Id = b.Id,
                ShortName = b.Company.ShortName
            })]
        })];
    }

    public async Task DeleteAgentAsync(int id, CancellationToken cancellationToken)
    {
        var agent = await context.Agents
            .Include(a => a.Company)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
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
