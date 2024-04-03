using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SOApiProject.Models;

namespace SOApiProject.Data;

public interface IMongoService
{
    Task AddToTagModelsAsync(List<TagModel> tagModels);
    Task<long> GetCollectionSize();
    Task<Dictionary<string, double>> GetTagsShare();
    Task<List<PagedTagModel>> GetSortedTags(string sortBy, bool ascending, int pageSize);
    void DropCollection();
    Task FetchDataToDatabase(int tagsAmount = 1000);
}

public class MongoService : IMongoService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TagModel> _collection;
    private readonly IOptions<MongoDBSettings> _mongoDbSettings;
    private readonly ILogger<MongoService> _logger;
    private readonly IDatabaseInitializer _databaseInitializer;

    public MongoService(IOptions<MongoDBSettings> mongoDbSettings, ILogger<MongoService> logger,
        IDatabaseInitializer databaseInitializer)
    {
        _mongoDbSettings = mongoDbSettings;
        _logger = logger;
        _databaseInitializer = databaseInitializer;
        var client = new MongoClient(_mongoDbSettings.Value.ConnectionURI);
        _database = client.GetDatabase(_mongoDbSettings.Value.DatabaseName);
        _collection = _database.GetCollection<TagModel>(_mongoDbSettings.Value.CollectionName);
    }

    public async Task<List<PagedTagModel>> GetSortedTags(string sortBy, bool ascending, int pageSize)
    {
        var tags = await GetTagsAsync(sortBy, ascending);

        var result = new List<PagedTagModel>();

        var pageCount = (int)Math.Ceiling((double)tags.Count / pageSize);

        for (int i = 0; i < pageCount; i++)
        {
            var page = tags.Skip(i * pageSize).Take(pageSize).ToList();
            result.Add(new PagedTagModel
            {
                Content = page,
                PageNumber = i + 1
            });
        }

        return result;
    }

    public async Task<Dictionary<string, double>> GetTagsShare()
    {
        var result = new Dictionary<string, double>();

        var tags = await GetTagsAsync();
        long totalTags = tags.Sum(tag => tag.Count);

        foreach (var tag in tags)
        {
            double share = (double)tag.Count * 100 / totalTags;
            result.Add(tag.Name, share);
        }

        return result;
    }

    public void DropCollection()
    {
        try
        {
            _database.DropCollectionAsync(_mongoDbSettings.Value.CollectionName);
            _logger.LogInformation($"Successfully dropped {_mongoDbSettings.Value.CollectionName} collection");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An error occurred while dropping {_mongoDbSettings.Value.CollectionName} collection.");
            throw;
        }
    }

    public async Task<long> GetCollectionSize()
    {
        var result = await _collection.CountDocumentsAsync(new BsonDocument());
        return result;
    }

    public async Task AddToTagModelsAsync(List<TagModel> tagModels)
    {
        try
        {
            foreach (var tag in tagModels)
            {
                if (tag.ID == null)
                {
                    tag.ID = ObjectId.GenerateNewId().ToString();
                }

                await _collection.InsertOneAsync(tag);
            }

            _logger.LogInformation($"Successfully added {tagModels.Count} tags to database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding tags to the database");
            throw;
        }
    }

    public async Task FetchDataToDatabase(int tagsAmount = 1000)
    {
        var currentCollectionSize = GetCollectionSize().Result;
        var missingTagsCount = tagsAmount - currentCollectionSize;

        if (missingTagsCount <= 0)
            return;

        var tags = _databaseInitializer.FetchTagsFromApi((int)missingTagsCount).Result;
        await AddToTagModelsAsync(tags);
    }

    private async Task<List<TagModel>> GetTagsAsync(string sortBy = "Name", bool ascending = true)
    {
        try
        {
            var filter = Builders<TagModel>.Filter.Empty;

            SortDefinition<TagModel> sortOptions;

            if (ascending)
            {
                sortOptions = Builders<TagModel>.Sort.Ascending(sortBy);
            }
            else
            {
                sortOptions = Builders<TagModel>.Sort.Descending(sortBy);
            }

            var result = await _collection.FindAsync(filter, new FindOptions<TagModel, TagModel>()
            {
                Sort = sortOptions
            }).Result.ToListAsync();

            _logger.LogInformation($"Successfully fetched {result.Count} tags from database");
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while fetching data from database");
            return new List<TagModel>();
        }
    }
}