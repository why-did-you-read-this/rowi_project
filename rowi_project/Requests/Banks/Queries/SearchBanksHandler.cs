using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Banks.Queries;

public class SearchBanksHandler(AppDbContext context, IMapper mapper) : IRequestHandler<SearchBanksQuery, List<BankDto>>
{
    public async Task<List<BankDto>> Handle(SearchBanksQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine("delme SearchBanksHandler"); // delme
        return await context.Banks
            .Where(b => EF.Functions.ILike(b.Company.ShortName, $"%{request.Name}%"))
            .OrderBy(b => b.Company.ShortName)
            .Take(10)
            .ProjectTo<BankDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
