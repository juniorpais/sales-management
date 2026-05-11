using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

public class GetProductProfile : Profile
{
    public GetProductProfile()
    {
        CreateMap<Product, GetProductResult>()
            .ForMember(d => d.RatingRate, o => o.MapFrom(s => s.Rating.Rate))
            .ForMember(d => d.RatingCount, o => o.MapFrom(s => s.Rating.Count));
    }
}
