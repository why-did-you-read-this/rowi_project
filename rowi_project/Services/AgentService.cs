using Microsoft.EntityFrameworkCore;
using Npgsql;
using rowi_project.Data;
using rowi_project.Models.Dtos;
using rowi_project.Models.Entities;

namespace rowi_project.Services;

public class AgentService : IAgentService
{
    private readonly AppDbContext _context;

    public AgentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAgentAsync(CreateAgentDto dto)
    {
        var company = new Company
        {
            ShortName = dto.ShortName,
            FullName = dto.FullName,
            Inn = dto.Inn,
            Kpp = dto.Kpp,
            Ogrn = dto.Ogrn,
            OgrnDateOfAssignment = dto.OgrnDateOfAssignment,
            RepName = dto.RepName,
            RepSurName = dto.RepSurname,
            RepPatronymic = dto.RepPatronymic,
            RepEmail = dto.RepEmail,
            RepPhoneNumber = dto.RepPhoneNumber,
        };

        var agent = new Agent
        {
            Important = dto.Important,
            Company = company
        };

        foreach (var bankId in dto.BankIds)
        {
            agent.AgentBanks.Add(new AgentBank
            {
                Agent = agent,
                BankId = bankId
            });
        }

        try
        {
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();

            return agent.Id;
        }
        // 23505 - unique_violation
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505") 
        {
            if (pgEx.ConstraintName == "uq_short_name")
                throw new ArgumentException("Компания с таким кратким наименованием уже существует");
            if (pgEx.ConstraintName == "uq_full_name")
                throw new ArgumentException("Компания с таким полным наименованием уже существует");
            if (pgEx.ConstraintName == "uq_inn")
                throw new ArgumentException("Компания с таким ИНН уже существует");
            if (pgEx.ConstraintName == "uq_ogrn")
                throw new ArgumentException("Компания с таким ОГРН уже существует");
            if (pgEx.ConstraintName == "uq_rep_email")
                throw new ArgumentException("Представитель с таким адресом электронной почты уже существует");
            if (pgEx.ConstraintName == "uq_rep_phone_number")
                throw new ArgumentException("Представитель с таким номером телефона уже существует");

            throw new ArgumentException("Нарушено уникальное ограничение");
        }

    }

    public async Task UpdateAgentAsync(UpdateAgentDto dto, CancellationToken cancellationToken)
    {
        var agent = await _context.Agents
            .Include(a => a.Company)
            .Include(a => a.AgentBanks)
            .FirstOrDefaultAsync(a => a.Id == dto.Id, cancellationToken);

        if (agent == null)
            throw new Exception("Агент не найден");

        var company = agent.Company;
        company.ShortName = dto.ShortName;
        company.FullName = dto.FullName;
        company.Inn = dto.Inn;
        company.Kpp = dto.Kpp;
        company.Ogrn = dto.Ogrn;
        company.OgrnDateOfAssignment = dto.OgrnDateOfAssignment;
        company.RepName = dto.RepName;
        company.RepSurName = dto.RepSurname;
        company.RepPatronymic = dto.RepPatronymic;
        company.RepEmail = dto.RepEmail;
        company.RepPhoneNumber = dto.RepPhoneNumber;

        agent.AgentBanks.Clear();
        foreach (var bankId in dto.BankIds)
        {
            agent.AgentBanks.Add(new AgentBank
            {
                AgentId = agent.Id,
                BankId = bankId
            });
        }

        agent.Important = dto.Important;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AgentDto?> GetAgentByIdAsync(int id)
    {
        var agent = await _context.Agents
            .Include(a => a.Company)
            .Include(a => a.AgentBanks)
                .ThenInclude(ab => ab.Bank)
                    .ThenInclude(b => b.Company)
                    .OrderBy(a => a.Id)
                    .FirstOrDefaultAsync(a => a.Id == id);
        //var t = await agent.FirstOrDefaultAsync(a => a.Id == id);


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
            Banks = agent.AgentBanks.Select(ab => new BankDto
            {
                Id = ab.Bank.Id,
                ShortName = ab.Bank.Company.ShortName
            }).ToList()
        };
    }
}
