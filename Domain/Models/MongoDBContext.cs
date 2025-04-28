
namespace API.Models;

using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

public class MongoDBContext
{
    public IMongoDatabase Database { get; }

    public MongoDBContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDBSettings:ConnectionString"]);
        Database = client.GetDatabase(configuration["MongoDBSettings:DatabaseName"]);
    }
}