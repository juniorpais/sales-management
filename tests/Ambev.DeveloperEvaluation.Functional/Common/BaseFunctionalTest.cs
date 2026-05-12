using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Common;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalWebApplicationFactory>
{
    protected readonly HttpClient Client;

    protected readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    protected BaseFunctionalTest(FunctionalWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected StringContent ToJsonContent(object obj) =>
        new(JsonSerializer.Serialize(obj, JsonOptions), Encoding.UTF8, "application/json");

    protected async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }
}
