using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
