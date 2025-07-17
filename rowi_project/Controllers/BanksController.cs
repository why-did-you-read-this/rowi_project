using MediatR;
using Microsoft.AspNetCore.Mvc;
using rowi_project.Models.Dtos;
using rowi_project.Requests.Banks.Queries;

namespace rowi_project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BankDto>>> GetBanks(CancellationToken cancellationToken)
    {
        var banks = await mediator.Send(new GetAllBanksQuery(), cancellationToken);
        return Ok(banks);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchBanks([FromQuery] string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            return Ok(new List<BankDto>());

        var banks = await mediator.Send(new SearchBanksQuery(name), cancellationToken);

        return Ok(banks);
    }
}
