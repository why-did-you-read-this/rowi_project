using MediatR;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;

namespace rowi_project.Requests.Agents.Commands;

public class DeleteAgentHandler(AppDbContext context) : IRequestHandler<DeleteAgentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = await context.Agents
            .Include(a => a.Company)
            .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Агент не найден");

        agent.Company.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

