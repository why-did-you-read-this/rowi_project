using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Banks.Queries;

public class GetAllBanksHandler (AppDbContext context, IMapper mapper) : IRequestHandler<GetAllBanksQuery,List<BankDto>>
{
    public async Task<List<BankDto>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        return await context.Banks.Include(b => b.Company)
            .ProjectTo<BankDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
