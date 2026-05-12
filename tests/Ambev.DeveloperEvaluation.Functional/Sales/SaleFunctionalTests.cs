using System.Net;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Functional.Common;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

/// <summary>
/// Functional tests for complete sale business flows.
/// Each test represents a real user journey through the system.
/// </summary>
public class SaleFunctionalTests : BaseFunctionalTest
{
    private static readonly Faker Faker = new();

    public SaleFunctionalTests(FunctionalWebApplicationFactory factory) : base(factory) { }

    private object BuildSaleRequest(string? saleNumber = null, int quantity = 2, decimal unitPrice = 100m) => new
    {
        saleNumber = saleNumber ?? $"SALE-{Faker.Random.Number(10000, 99999)}",
        date = DateTime.UtcNow.ToString("o"),
        customerId = Guid.NewGuid(),
        customerName = Faker.Name.FullName(),
        branchId = Guid.NewGuid(),
        branchName = Faker.Company.CompanyName(),
        items = new[]
        {
            new
            {
                productId = Guid.NewGuid(),
                productName = Faker.Commerce.ProductName(),
                quantity,
                unitPrice
            }
        }
    };

    [Fact(DisplayName = "Full sale lifecycle: create, retrieve, update, cancel")]
    public async Task FullSaleLifecycle_CreateUpdateCancel_Succeeds()
    {
        // 1. Create
        var createRequest = BuildSaleRequest(quantity: 3, unitPrice: 50m);
        var createResponse = await Client.PostAsync("/api/sales", ToJsonContent(createRequest));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(createResponse);
        created!.Data.Should().NotBeNull();
        var saleId = created.Data!.Id;

        // 2. Retrieve and verify no discount for qty < 4
        var getResponse = await Client.GetAsync($"/api/sales/{saleId}");
        var sale = await DeserializeAsync<ApiResponseWithData<GetSaleResult>>(getResponse);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        sale!.Data!.Items.First().Discount.Should().Be(0m);
        sale.Data.TotalAmount.Should().Be(3 * 50m);

        // 3. Update
        var updateRequest = new
        {
            date = DateTime.UtcNow.AddDays(1).ToString("o"),
            customerId = Guid.NewGuid(),
            customerName = "Updated Customer",
            branchId = Guid.NewGuid(),
            branchName = "Updated Branch"
        };
        var updateResponse = await Client.PutAsync($"/api/sales/{saleId}", ToJsonContent(updateRequest));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updated = await DeserializeAsync<ApiResponseWithData<UpdateSaleResult>>(updateResponse);
        updated!.Data!.CustomerName.Should().Be("Updated Customer");

        // 4. Cancel
        var cancelResponse = await Client.DeleteAsync($"/api/sales/{saleId}");
        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 5. Verify cancelled sale cannot be cancelled again
        var cancelAgainResponse = await Client.DeleteAsync($"/api/sales/{saleId}");
        cancelAgainResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Discount tiers flow: create sales with different quantities and verify discounts")]
    public async Task DiscountTiers_AllTiers_AppliedCorrectly()
    {
        // Tier 1: qty < 4 → no discount
        var noDiscountRequest = BuildSaleRequest(quantity: 3, unitPrice: 100m);
        var noDiscountResponse = await Client.PostAsync("/api/sales", ToJsonContent(noDiscountRequest));
        var noDiscount = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(noDiscountResponse);
        noDiscount!.Data!.Items.First().Discount.Should().Be(0m);
        noDiscount.Data.TotalAmount.Should().Be(300m);

        // Tier 2: qty 4-9 → 10% discount
        var tenPercentRequest = BuildSaleRequest(quantity: 7, unitPrice: 100m);
        var tenPercentResponse = await Client.PostAsync("/api/sales", ToJsonContent(tenPercentRequest));
        var tenPercent = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(tenPercentResponse);
        tenPercent!.Data!.Items.First().Discount.Should().Be(0.10m);
        tenPercent.Data.TotalAmount.Should().Be(7 * 100m * 0.90m);

        // Tier 3: qty 10-20 → 20% discount
        var twentyPercentRequest = BuildSaleRequest(quantity: 12, unitPrice: 100m);
        var twentyPercentResponse = await Client.PostAsync("/api/sales", ToJsonContent(twentyPercentRequest));
        var twentyPercent = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(twentyPercentResponse);
        twentyPercent!.Data!.Items.First().Discount.Should().Be(0.20m);
        twentyPercent.Data.TotalAmount.Should().Be(12 * 100m * 0.80m);

        // Tier 4: qty > 20 → rejected
        var invalidRequest = BuildSaleRequest(quantity: 21, unitPrice: 100m);
        var invalidResponse = await Client.PostAsync("/api/sales", ToJsonContent(invalidRequest));
        invalidResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "Duplicate sale number flow: second creation with same number fails")]
    public async Task DuplicateSaleNumber_SecondCreation_Fails()
    {
        var saleNumber = $"SALE-UNIQUE-{Faker.Random.Number(10000, 99999)}";

        var firstRequest = BuildSaleRequest(saleNumber: saleNumber);
        var firstResponse = await Client.PostAsync("/api/sales", ToJsonContent(firstRequest));
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondRequest = BuildSaleRequest(saleNumber: saleNumber);
        var secondResponse = await Client.PostAsync("/api/sales", ToJsonContent(secondRequest));
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "List sales flow: create multiple sales and verify pagination")]
    public async Task ListSales_WithPagination_ReturnsCorrectPage()
    {
        // Create 3 sales
        for (var i = 0; i < 3; i++)
        {
            var request = BuildSaleRequest();
            await Client.PostAsync("/api/sales", ToJsonContent(request));
        }

        // Get page 1 with size 2
        var listResponse = await Client.GetAsync("/api/sales?_page=1&_size=2");
        var list = await DeserializeAsync<ApiResponseWithData<GetSalesResult>>(listResponse);

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        list!.Data.Should().NotBeNull();
        list.Data!.Data.Count().Should().BeLessOrEqualTo(2);
        list.Data.TotalItems.Should().BeGreaterOrEqualTo(3);
        list.Data.CurrentPage.Should().Be(1);
    }

    [Fact(DisplayName = "Update cancelled sale flow: updating a cancelled sale fails")]
    public async Task UpdateCancelledSale_Fails()
    {
        // Create and cancel
        var createRequest = BuildSaleRequest(quantity: 2);
        var createResponse = await Client.PostAsync("/api/sales", ToJsonContent(createRequest));
        var created = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(createResponse);
        var saleId = created!.Data!.Id;

        await Client.DeleteAsync($"/api/sales/{saleId}");

        // Try to update cancelled sale
        var updateRequest = new
        {
            date = DateTime.UtcNow.ToString("o"),
            customerId = Guid.NewGuid(),
            customerName = "Should Fail",
            branchId = Guid.NewGuid(),
            branchName = "Should Fail"
        };
        var updateResponse = await Client.PutAsync($"/api/sales/{saleId}", ToJsonContent(updateRequest));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
