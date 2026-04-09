using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Aldaman.Web.Tests;

public class PublicControllersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PublicControllersTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/cs")]
    [InlineData("/cs/Home/Privacy")]
    [InlineData("/cs/blog")]
    [InlineData("/cs/contact")]
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
    [InlineData("/cs/page/neexistujici-stranka")]
    [InlineData("/cs/blog/neexistujici-clanek")]
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
