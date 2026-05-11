using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<UpdateUserResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UpdateUserResult>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateUserValidator();
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
        if (user is null)
            return Result.Fail<UpdateUserResult>($"User with ID '{command.Id}' not found.");

        user.Username = command.Username;
        user.Email = command.Email;
        user.Phone = command.Phone;
        user.Status = command.Status;
        user.Role = command.Role;
        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRepository.UpdateAsync(user, cancellationToken);
        return Result.Ok(_mapper.Map<UpdateUserResult>(updated));
    }
}
