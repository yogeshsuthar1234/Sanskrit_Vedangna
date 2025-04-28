using MongoDB.Driver;
using Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Persistence.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDBSettings:ConnectionString"]);
        _database = client.GetDatabase(config["MongoDBSettings:DatabaseName"]);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<Progress> Progress => _database.GetCollection<Progress>("Progress");
}