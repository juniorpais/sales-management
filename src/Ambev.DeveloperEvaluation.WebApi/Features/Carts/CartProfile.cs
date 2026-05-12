using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.Requests;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<CreateCartRequest, CreateCartCommand>();
        CreateMap<CreateCartItemRequest, CreateCartItemCommand>();
        CreateMap<UpdateCartRequest, UpdateCartCommand>();
        CreateMap<UpdateCartItemRequest, UpdateCartItemCommand>();
    }
}
