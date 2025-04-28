using Domain.Models;
using MongoDB.Driver;
using Persistence.Context;

namespace Persistence.Repositories;

public class ProgressRepository
{
    private readonly IMongoCollection<Progress> _progress;

    public ProgressRepository(MongoDbContext context)
    {
        _progress = context.Progress;
    }
// In ProgressRepository.cs
public async Task UpdateAsync(Progress progress)
{
    await _progress.ReplaceOneAsync(
        p => p.Id == progress.Id, 
        progress
    );
}
    public async Task<Progress> GetUserProgress(string userId)
    {
        return await _progress.Find(p => p.UserId == userId).FirstOrDefaultAsync();
    }

    // public async Task UpdateCourseProgress(string userId, CourseProgress course)
    // {
    //     var filter = Builders<Progress>.Filter.Eq(p => p.UserId, userId);
    //     var update = Builders<Progress>.Update
    //         .Set(p => p.LastUpdated, DateTime.UtcNow)
    //         .AddToSet(p => p.CourseProgress, course);

    //     await _progress.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    // }

    // public async Task UpdateServiceUsage(string userId, ServiceUsage service)
    // {
    //     var filter = Builders<Progress>.Filter.Eq(p => p.UserId, userId);
    //     var update = Builders<Progress>.Update
    //         .Set(p => p.LastUpdated, DateTime.UtcNow)
    //         .AddToSet(p => p.ServiceUsage, service);

    //     await _progress.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    // }private readonly IMongoCollection<Progress> _progressCollection;

    

    public async Task<Progress> GetByUserIdAsync(string userId)
    {
        return await _progress
            .Find(p => p.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task InsertAsync(Progress progress)
    {
        await _progress.InsertOneAsync(progress);
    }
}