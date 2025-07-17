using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using rowi_project.Data;
using rowi_project.Exceptions;
using rowi_project.Models.Entities;

namespace rowi_project.Requests.Agents.Commands;

public class CreateAgentHandler(AppDbContext context, IMapper mapper) : IRequestHandler<CreateAgentCommand, int>
{
    public async Task<int> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

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
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            return DbExceptionHandler.HandleUniqueViolation(pgEx);
        }
    }
}