using rowi_project.Models.Dtos;

namespace rowi_project.Services;

public interface IBankService
{
    Task<List<BankDto>> GetAllBanksAsync(CancellationToken cancellationToken);
    Task<List<BankDto>> SearchBanksAsync(string name, CancellationToken cancellationToken);
}
