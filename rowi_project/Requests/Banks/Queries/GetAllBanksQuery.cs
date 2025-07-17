using MediatR;
using rowi_project.Models.Dtos;

namespace rowi_project.Requests.Banks.Queries;

public record GetAllBanksQuery () : IRequest<List<BankDto>>;
