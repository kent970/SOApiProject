﻿using MongoDB.Driver;
using MongoDB.Entities;

namespace SOApiProject.Data;

public class MongoDBSettings
{
    public string ConnectionURI { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}