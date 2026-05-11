using AutoMapper;
using FluentResults;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, Result<GetUsersResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetUsersResult>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _userRepository.GetAllAsync(query.Page, query.Size, query.Order, cancellationToken);

        return Result.Ok(new GetUsersResult
        {
            Data = _mapper.Map<IEnumerable<GetUserResult>>(items),
            TotalItems = totalCount,
            CurrentPage = query.Page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.Size)
        });
    }
}
