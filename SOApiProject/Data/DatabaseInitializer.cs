using System.Text.Json;
using SOApiProject.Models;

namespace SOApiProject.Data;

public interface IDatabaseInitializer
{
    Task InitDb(int tagsAmount = 1000);
}

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly HttpClient _httpClient;
    private readonly IMongoService _mongoService;
    private readonly ILogger<IDatabaseInitializer> _logger;

    public DatabaseInitializer(HttpClient httpClient,
        IMongoService mongoService, ILogger<IDatabaseInitializer> logger)
    {
        _httpClient = httpClient;
        _mongoService = mongoService;
        _logger = logger;
    }

    public async Task InitDb(int tagsAmount = 1000)
    {
        var collectionSize = _mongoService.GetCollectionSize().Result;
        var missingTagsCount = tagsAmount - collectionSize;

        if (missingTagsCount <= 0)
            return;

        try
        {
            int pageCount = (int)Math.Ceiling((double)missingTagsCount / 100);

            var tagModels = new List<TagModel>();


            for (int i = 1; i <= pageCount; i++)
            {
                string apiUrl = $"?page={i}&pagesize=100&order=desc&sort=popular&site=stackoverflow";

                var response = await _httpClient.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to fetch data from API. Status code: {response.StatusCode}");

                var contentStream = await response.Content.ReadAsStreamAsync();

                using (var gzip = new System.IO.Compression.GZipStream(contentStream,
                           System.IO.Compression.CompressionMode.Decompress))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = JsonSerializer.Deserialize<Response>(gzip, options);

                    if (result != null)
                    {
                        tagModels.AddRange(result.Items);
                    }
                }

                _logger.LogInformation("Successfully fetched data from API");
            }

            await _mongoService.AddToTagModelsAsync(tagModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}