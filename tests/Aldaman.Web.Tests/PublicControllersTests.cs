using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Aldaman.Web.Tests;

public class PublicControllersTests : IClassFixture<WebApplicationFactory<Program>> // TODO...
{
    private readonly WebApplicationFactory<Program> _factory;

    public PublicControllersTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/clanky")]
    [InlineData("/kontakt")]
    public async Task Get_EndpointsReturnSuccessAndHtmlContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Theory]
    [InlineData("/stranka/neexistujici-stranka")]
    [InlineData("/clanky/neexistujici-clanek")]
    public async Task Get_DetailEndpoints_ReturnNotFoundForMissingItems(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
