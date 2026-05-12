using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static CreateSaleCommand GenerateValidCommand() => new()
    {
        SaleNumber = $"SALE-{Faker.Random.Number(1000, 9999)}",
        Date = Faker.Date.Recent(),
        CustomerId = Guid.NewGuid(),
        CustomerName = Faker.Name.FullName(),
        BranchId = Guid.NewGuid(),
        BranchName = Faker.Company.CompanyName(),
        Items =
        [
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = Faker.Commerce.ProductName(),
                Quantity = Faker.Random.Int(1, 3),
                UnitPrice = Faker.Random.Decimal(10, 500)
            }
        ]
    };

    public static CreateSaleCommand GenerateCommandWithQuantityAbove20() => new()
    {
        SaleNumber = $"SALE-{Faker.Random.Number(1000, 9999)}",
        Date = Faker.Date.Recent(),
        CustomerId = Guid.NewGuid(),
        CustomerName = Faker.Name.FullName(),
        BranchId = Guid.NewGuid(),
        BranchName = Faker.Company.CompanyName(),
        Items =
        [
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                ProductName = Faker.Commerce.ProductName(),
                Quantity = 21,
                UnitPrice = 100m
            }
        ]
    };
}
