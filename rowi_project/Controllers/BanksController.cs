using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rowi_project.Models.Dtos;
using rowi_project.Services;

namespace rowi_project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController(IBankService bankService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BankDto>>> GetBanks(CancellationToken cancellationToken)
    {
        var banks = await bankService.GetAllBanksAsync(cancellationToken);
        return Ok(banks);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchBanks([FromQuery] string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
            return Ok(new List<BankDto>());

        var banks = await bankService.SearchBanksAsync(query, cancellationToken);
        return Ok(banks);
    }
}
