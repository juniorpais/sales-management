using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductProfile : Profile
{
    public CreateProductProfile()
    {
        CreateMap<Product, CreateProductResult>()
            .ForMember(d => d.RatingRate, o => o.MapFrom(s => s.Rating.Rate))
            .ForMember(d => d.RatingCount, o => o.MapFrom(s => s.Rating.Count));
    }
}
