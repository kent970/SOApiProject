using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using SOApiProject.Models;
using ZstdSharp.Unsafe;

namespace SOApiProject.Data;

public interface IMongoService
{
    Task<List<TagModel>> GetTagsAsync();
    Task DropCollection();
    Task AddToTagModelsAsync(List<TagModel> tagModels);
}
public class MongoService : IMongoService
{
    //todo wyabstrachowac interfejs
    private readonly IMongoCollection<TagModel> _tagCollection;
    private readonly string connectionString;
    private readonly MongoClient client;
    private readonly IMongoDatabase database;
    private readonly IMongoCollection<TagModel> collection;
    private readonly string collectionName = "tags2";

    public MongoService(IOptions<MongoDBSettings> mongoDbSettings)
    {
        //todo wstrzykiwac z appsetingsow
        connectionString = "mongodb://root:mongopw@localhost:27017";
        client = new MongoClient(connectionString);
        database = client.GetDatabase("tagsDatabase");
        collection = database.GetCollection<TagModel>(collectionName);
    }
    
    public async Task<List<TagModel>> GetTagsAsync()
    {
        return await collection.Find(_ => true).Limit(35).ToListAsync();
    }

    public async Task DropCollection()
    {
        await database.DropCollectionAsync(collectionName);
    }

    
    public async Task AddToTagModelsAsync(List<TagModel> tagModels)
    {
        foreach (var tag in tagModels)
        {
            if (tag.ID == null)
            {
                tag.ID = ObjectId.GenerateNewId().ToString();
            }

            await collection.InsertOneAsync(tag);
        }
    }
}