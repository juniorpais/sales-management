using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers;

public class GetUsersQuery : IRequest<Result<GetUsersResult>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
}
