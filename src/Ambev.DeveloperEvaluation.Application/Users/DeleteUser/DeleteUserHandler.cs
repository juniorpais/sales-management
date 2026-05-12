using MediatR;
using FluentResults;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Result<DeleteUserResponse>>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<DeleteUserResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _userRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            return Result.Fail<DeleteUserResponse>($"User with ID {request.Id} not found");

        return Result.Ok(new DeleteUserResponse { Success = true });
    }
}
