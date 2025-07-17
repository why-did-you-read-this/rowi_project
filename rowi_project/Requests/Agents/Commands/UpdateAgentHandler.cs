using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;

namespace rowi_project.Requests.Agents.Commands;

public class UpdateAgentHandler(AppDbContext context, IMapper mapper) : IRequestHandler<UpdateAgentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = await context.Agents
            .Include(a => a.Company)
            .Include(a => a.Banks)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new ArgumentException("Агент не найден");

        var dto = request.Dto;

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
        return Unit.Value;
    }
}

