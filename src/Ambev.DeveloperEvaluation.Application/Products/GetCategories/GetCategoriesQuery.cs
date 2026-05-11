using FluentResults;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetCategories;

public class GetCategoriesQuery : IRequest<Result<IEnumerable<string>>>
{
}
