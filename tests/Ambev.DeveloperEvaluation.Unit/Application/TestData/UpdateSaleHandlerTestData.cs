using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateSaleHandlerTestData
{
    private static readonly Faker Faker = new();

    public static UpdateSaleCommand GenerateValidCommand(Guid? saleId = null) => new()
    {
        Id = saleId ?? Guid.NewGuid(),
        Date = Faker.Date.Recent(),
        CustomerId = Guid.NewGuid(),
        CustomerName = Faker.Name.FullName(),
        BranchId = Guid.NewGuid(),
        BranchName = Faker.Company.CompanyName()
    };
}
