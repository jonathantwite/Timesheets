using Microsoft.AspNetCore.Mvc.Testing;
using TimeAdder.Api.Contracts.Responses;

namespace TimeAdder.Api.Tests.Endpoints;

public class TimeEndpointsTests
{
    public TimeEndpointsTests()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _factory.Dispose();
    }

    [Test]
    public async Task Get_ReturnsListOfItems()
    {
        var response = await _client.GetAsync("/Time");
        response.EnsureSuccessStatusCode();
        Assert.DoesNotThrowAsync(async() => System.Text.Json.JsonSerializer.Deserialize<List<TimeResponse>>(await response.Content.ReadAsStringAsync()));
    }

    public void Get_ReturnsEmptyListIfNoItemsAvailable()
    {

    }
}
