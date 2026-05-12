using Ambev.DeveloperEvaluation.Common.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.IoC.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICartRepository, CartRepository>();
        
        builder.Services.ConfigureRebus(builder.Configuration);
    }
}

public static class RebusConfigurationExtensions
{
    public static IServiceCollection ConfigureRebus(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq");

        if (string.IsNullOrWhiteSpace(rabbitMqConnectionString))
        {
            services.AddScoped<IEventPublisher, LogEventPublisher>();
            return services;
        }

        services.AddRebus(configure => configure
            .Transport(t => t.UseRabbitMq(rabbitMqConnectionString, "sales-management"))
            .Routing(r => r.TypeBased().MapAssemblyOf<IDomainEvent>("sales-management")));

        services.AddScoped<IEventPublisher, RebusEventPublisher>();
        services.AddScoped<LogEventPublisher>();

        return services;
    }
}
