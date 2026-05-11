using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetCategories;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProducts;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.Requests;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

[ApiController]
[Route("api/products")]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateProductResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateProductCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return BadRequest(result.Errors.First().Message);

        return Created(string.Empty, new { }, result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductQuery { Id = id }, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] int _page = 1, [FromQuery] int _size = 10, [FromQuery] string? _order = null, [FromQuery] string? category = null, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetProductsQuery { Page = _page, Size = _size, Order = _order, Category = category }, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductsResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCategory([FromRoute] string category, [FromQuery] int _page = 1, [FromQuery] int _size = 10, [FromQuery] string? _order = null, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetProductsQuery { Page = _page, Size = _size, Order = _order, Category = category }, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateProductResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateProductCommand>(request);
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken);
        return HandleResult(result);
    }
}
