using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using SOApiProject.Models;

namespace SOApiProject.Data;

public class MongoService
{
    private readonly IMongoCollection<TagModel> _tagCollection;
    private readonly IMongoDatabase database;

    public MongoService(IOptions<MongoDBSettings> mongoDbSettings)
    {
    }

    public async Task<List<TagModel>> GetAsync()
    {
        string connectionString = "mongodb://root:mongopw@localhost:27017";
        string connectionTry = "mongodb://root:mongopw@localhost";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("tagsDatabase");
        var collection = database.GetCollection<TagModel>("tags2");

        return await collection.Find(_ => true).Limit(35).ToListAsync();
    }

    public async Task CreateAsync(TagModel tagModels)
    {
    }

    public async Task AddToTagModelsAsync(List<TagModel> tagModels)
    {
        string connectionString = "mongodb://root:mongopw@localhost:27017";
        string connectionTry = "mongodb://root:mongopw@localhost";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("tagsDatabase");
        var collection = database.GetCollection<TagModel>("tags2");
        foreach (var tag in tagModels)
        {
            if (tag.ID == null)
            {
                tag.ID = ObjectId.GenerateNewId().ToString();
            }

            await collection.InsertOneAsync(tag);
        }
    }

    public async Task DeleteAsync(string id)
    {
    }
}