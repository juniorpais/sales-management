using System.Net;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Integration.Common;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

public class SalesIntegrationTests : BaseIntegrationTest
{
    private static readonly Faker Faker = new();

    public SalesIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    private object GenerateSaleRequest(int quantity = 2) => new
    {
        saleNumber = $"SALE-{Faker.Random.Number(10000, 99999)}",
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
                unitPrice = 100.00m
            }
        }
    };

    [Fact(DisplayName = "POST /api/sales with valid data returns 201")]
    public async Task CreateSale_ValidRequest_Returns201()
    {
        var request = GenerateSaleRequest(2);

        var response = await Client.PostAsync("/api/sales", ToJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "POST /api/sales with quantity 5 applies 10 percent discount")]
    public async Task CreateSale_Quantity5_Applies10PercentDiscount()
    {
        var request = GenerateSaleRequest(5);

        var response = await Client.PostAsync("/api/sales", ToJsonContent(request));
        var body = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(response);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body!.Data!.Items.First().Discount.Should().Be(0.10m);
        body.Data.Items.First().TotalAmount.Should().Be(5 * 100m * 0.90m);
    }

    [Fact(DisplayName = "POST /api/sales with quantity 15 applies 20 percent discount")]
    public async Task CreateSale_Quantity15_Applies20PercentDiscount()
    {
        var request = GenerateSaleRequest(15);

        var response = await Client.PostAsync("/api/sales", ToJsonContent(request));
        var body = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(response);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        body!.Data!.Items.First().Discount.Should().Be(0.20m);
        body.Data.Items.First().TotalAmount.Should().Be(15 * 100m * 0.80m);
    }

    [Fact(DisplayName = "POST /api/sales with quantity 21 returns 400")]
    public async Task CreateSale_Quantity21_Returns400()
    {
        var request = GenerateSaleRequest(21);

        var response = await Client.PostAsync("/api/sales", ToJsonContent(request));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(DisplayName = "GET /api/sales/{id} for existing sale returns 200")]
    public async Task GetSale_ExistingSale_Returns200()
    {
        var createRequest = GenerateSaleRequest(2);
        var createResponse = await Client.PostAsync("/api/sales", ToJsonContent(createRequest));
        var created = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(createResponse);

        var response = await Client.GetAsync($"/api/sales/{created!.Data!.Id}");
        var body = await DeserializeAsync<ApiResponseWithData<GetSaleResult>>(response);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Data!.Id.Should().Be(created.Data.Id);
    }

    [Fact(DisplayName = "GET /api/sales/{id} for non-existing sale returns 404")]
    public async Task GetSale_NonExistingSale_Returns404()
    {
        var response = await Client.GetAsync($"/api/sales/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(DisplayName = "GET /api/sales returns paginated list")]
    public async Task GetSales_Returns200WithPaginatedData()
    {
        var response = await Client.GetAsync("/api/sales?_page=1&_size=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "DELETE /api/sales/{id} for existing sale cancels it")]
    public async Task CancelSale_ExistingSale_Returns200()
    {
        var createRequest = GenerateSaleRequest(2);
        var createResponse = await Client.PostAsync("/api/sales", ToJsonContent(createRequest));
        var created = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(createResponse);

        var response = await Client.DeleteAsync($"/api/sales/{created!.Data!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "DELETE /api/sales/{id} twice returns 400")]
    public async Task CancelSale_Twice_Returns400()
    {
        var createRequest = GenerateSaleRequest(2);
        var createResponse = await Client.PostAsync("/api/sales", ToJsonContent(createRequest));
        var created = await DeserializeAsync<ApiResponseWithData<CreateSaleResult>>(createResponse);

        await Client.DeleteAsync($"/api/sales/{created!.Data!.Id}");
        var response = await Client.DeleteAsync($"/api/sales/{created.Data.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
