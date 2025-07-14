using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using rowi_project.Data;
using rowi_project.Models.Dtos;

namespace rowi_project.Services;

public class BankService(AppDbContext context, IMapper mapper) : IBankService
{
    public async Task<List<BankDto>> GetAllBanksAsync(CancellationToken cancellationToken)
    {
        return await context.Banks.Include(b => b.Company)
            .ProjectTo<BankDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BankDto>> SearchBanksAsync(string name, CancellationToken cancellationToken)
    {
        return await context.Banks
            .Where(b => EF.Functions.ILike(b.Company.ShortName, $"%{name}%"))
            .OrderBy(b => b.Company.ShortName)
            .Take(10)
            .ProjectTo<BankDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
