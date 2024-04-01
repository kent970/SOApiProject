using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Entities;

namespace SOApiProject.Models;

public class TagModel : Entity
{

    
    [BsonElement("Name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }


    [BsonElement("HasSynonyms")]
    [JsonPropertyName("has_synonyms")]
    public bool HasSynonyms { get; set; }

    [BsonElement("IsModeratorOnly")]
    [JsonPropertyName("is_moderator_only")]
    public bool IsModeratorOnly { get; set; }

    [BsonElement("IsRequired")]
    [JsonPropertyName("is_required")]
    public bool IsRequired { get; set; }

    [BsonElement("Count")]
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class Response
{
    //TODO moze zmienc na ienumerable
    public List<TagModel> Items { get; set; }
    
}

public class PagedTagModel
{
    public List<TagModel> Content { get; set; }
    public int PageNumber { get; set; }
}