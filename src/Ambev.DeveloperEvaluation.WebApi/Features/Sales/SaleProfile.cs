using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Requests;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemCommand>();
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
    }
}
