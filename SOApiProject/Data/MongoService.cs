using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SOApiProject.Models;

namespace SOApiProject.Data;

public interface IMongoService
{
    Task<List<TagModel>> GetTagsAsync(string sortBy = "Name", bool ascending = true);
    Task DropCollection();
    Task AddToTagModelsAsync(List<TagModel> tagModels);
    Task<long> GetCollectionSize();
}

public class MongoService : IMongoService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TagModel> _collection;
    private readonly IOptions<MongoDBSettings> _mongoDbSettings;
    private readonly ILogger<MongoService> _logger;

    public MongoService(IOptions<MongoDBSettings> mongoDbSettings, ILogger<MongoService> logger)
    {
        _mongoDbSettings = mongoDbSettings;
        _logger = logger;
        var client = new MongoClient(_mongoDbSettings.Value.ConnectionURI);
        _database = client.GetDatabase(_mongoDbSettings.Value.DatabaseName);
        _collection = _database.GetCollection<TagModel>(_mongoDbSettings.Value.CollectionName);
    }

    public async Task<List<TagModel>> GetTagsAsync(string sortBy = "Name", bool ascending = true)
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

            _logger.LogInformation("Successfully fetched tags from database");
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while fetching data from database");
            return new List<TagModel>();
        }
    }

    public async Task DropCollection()
    {
        try
        {
            await _database.DropCollectionAsync(_mongoDbSettings.Value.CollectionName);
            _logger.LogInformation("Successfully dropped collection");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while dropping the collection.");
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

            _logger.LogInformation("Successfully added tags to database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding tags to the database");
            throw;
        }
    }
}