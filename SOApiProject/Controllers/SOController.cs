using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SOApiProject.Models;

namespace SOApiProject.Controllers;

[ApiController]
[Route("api/so")]
public class SOController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SOController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<Response> GetTags(int pageNum = 1, int pageSize = 100)
    {
        var httpClient = _httpClientFactory.CreateClient("SOClient");


        string apiBaseUrl = _configuration["ApiSettings:BaseUrl"];

        string apiUrl = $"{apiBaseUrl}?page={pageNum}&pagesize={pageSize}&order=desc&sort=popular&site=stackoverflow";


        var response = await httpClient.GetAsync(apiUrl);

        response.EnsureSuccessStatusCode();

        if (response.IsSuccessStatusCode)
        {
            var contentStream = await response.Content.ReadAsStreamAsync();
            using (var gzip = new System.IO.Compression.GZipStream(contentStream,
                       System.IO.Compression.CompressionMode.Decompress))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<Response>(gzip, options);

                return result;
            }
        }
        else
        {
            return null;
        }
    }
}