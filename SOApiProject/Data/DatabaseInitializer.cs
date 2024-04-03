using System.Text.Json;
using SOApiProject.Models;

namespace SOApiProject.Data;

public interface IDatabaseInitializer
{
    Task<List<TagModel>> FetchTagsFromApi(int tagsAmount = 1000);
}

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ILogger<IDatabaseInitializer> _logger;
    private readonly IApiService _apiService;

    public DatabaseInitializer(ILogger<IDatabaseInitializer> logger, IApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<List<TagModel>> FetchTagsFromApi(int missingTagsCount)
    {
        try
        {
            int pageCount = (int)Math.Ceiling((double)missingTagsCount / 100);

            var tagModels = new List<TagModel>();


            for (int i = 1; i <= pageCount; i++)
            {
                var response = await _apiService.GetResponse(i);

                response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Failed to fetch data from API. Status code: {response.StatusCode}");

                var contentStream = await response.Content.ReadAsStreamAsync();

                using (var gzip = new System.IO.Compression.GZipStream(contentStream, System.IO.Compression.CompressionMode.Decompress))
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

            return tagModels;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}