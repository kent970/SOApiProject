using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using SOApiProject.Data;
using SOApiProject.Models;
using IConfiguration = Castle.Core.Configuration.IConfiguration;

namespace SOApiTests.Unit;

public class ApiServiceTests
{
    private readonly IConfigurationRoot _configuration;

    public ApiServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }

    [Fact]
    public async Task GetDataFromApi_ReturnsData()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_configuration["BaseUrl"])
        };
        var apiService = new ApiService(httpClient);
        int page = 1;

        // Act
        var data = await apiService.GetResponse(page);

        // Assert
        data.Should().NotBeNull();
        data.Should().BeOfType<HttpResponseMessage>();
    }

    [Fact]
    public async Task GetDataFromApi_ResponseCanBeDeserialized()
    {
        // Arrange
        
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_configuration["BaseUrl"])
        };
        var apiService = new ApiService(httpClient);
        int page = 1;

        // Act
        var data = await apiService.GetResponse(page);
        var contentStream = await data.Content.ReadAsStreamAsync();
        var gzip = new System.IO.Compression.GZipStream(contentStream, System.IO.Compression.CompressionMode.Decompress);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var result = JsonSerializer.Deserialize<Response>(gzip, options);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
    }
}