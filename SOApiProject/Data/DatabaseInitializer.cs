using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SOApiProject.Models;

namespace SOApiProject.Data;

public class DatabaseInitializer
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public DatabaseInitializer(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task InitDb()
    {
        await DB.InitAsync("tags", MongoClientSettings
            .FromConnectionString("mongodb://root:mongopw@localhost"));

        await DB.Index<TagModel>()
            .Key(x => x.Name, KeyType.Text).CreateAsync();

        var count = await DB.CountAsync<TagModel>();

       //
       //using var scope = app.Services.CreateScope();
        
        var items = GetTags();

        if (items.Result.Count > 0) await DB.SaveAsync(items.Result);
    }
    public async Task<List<TagModel>> GetTags(int startPage = 1, int pageSize = 100, int pageCount = 1)
    {
        var tagModels = new List<TagModel>();
        var pageNum = startPage;
        
        var httpClient = _httpClientFactory.CreateClient("SOClient");

        
        string apiBaseUrl = _configuration["BaseUrl"];

        for (int i = pageNum; i <= pageCount; i++)
        {
            string apiUrl = $"{apiBaseUrl}?page={startPage}&pagesize={pageSize}&order=desc&sort=popular&site=stackoverflow";
           
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
           
                    tagModels.AddRange(result.Items);
                }
            }
        }
    
        return tagModels;
    }
}