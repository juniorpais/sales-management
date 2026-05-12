using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker Faker = new();

    public static Sale GenerateValidSale() =>
        Sale.Create(
            saleNumber: $"SALE-{Faker.Random.Number(1000, 9999)}",
            date: Faker.Date.Recent(),
            customerId: Guid.NewGuid(),
            customerName: Faker.Name.FullName(),
            branchId: Guid.NewGuid(),
            branchName: Faker.Company.CompanyName()
        );

    public static (Guid productId, string productName, int quantity, decimal unitPrice) GenerateValidItem(int quantity = 2) =>
        (Guid.NewGuid(), Faker.Commerce.ProductName(), quantity, Faker.Random.Decimal(10, 500));
}
