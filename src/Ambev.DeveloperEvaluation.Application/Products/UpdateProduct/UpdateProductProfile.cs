using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductProfile : Profile
{
    public UpdateProductProfile()
    {
        CreateMap<Product, UpdateProductResult>()
            .ForMember(d => d.RatingRate, o => o.MapFrom(s => s.Rating.Rate))
            .ForMember(d => d.RatingCount, o => o.MapFrom(s => s.Rating.Count));
    }
}
