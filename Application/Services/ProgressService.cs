using Domain.Models;
using Persistence.Repositories;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services;

public class ProgressService
{
    private readonly ProgressRepository _progressRepo;

    public ProgressService(ProgressRepository progressRepo)
    {
        _progressRepo = progressRepo;
    }

    public async Task<Progress> GetUserProgress(string userId)
    {
        // Use repository method to fetch progress
        var progress = await _progressRepo.GetByUserIdAsync(userId);

        if (progress == null)
        {
            // Create default progress for new users
            progress = new Progress
            {
                UserId = userId,
                LastUpdated = DateTime.UtcNow
            };
            await _progressRepo.InsertAsync(progress);
        }

        // Merge with expected courses/services/vedas
        return MergeWithDefaultProgress(progress);
    }


// In ProgressService.cs
public async Task UpdateVedaSubtype(string userId, string vedaName, string subtypeName, bool result)
{
    var progress = await GetUserProgress(userId);
    var veda = progress.Vedas.FirstOrDefault(v => v.VedaName == vedaName);

    if (veda == null) return;

    var subtype = veda.Subtypes.FirstOrDefault(s => s.SubtypeName == subtypeName);
    if (subtype == null)
    {
        subtype = new VedaSubtype { SubtypeName = subtypeName };
        veda.Subtypes.Add(subtype);
    }

    subtype.Result = result;
    subtype.LastAttempted = DateTime.UtcNow;
    veda.LastAccessed = DateTime.UtcNow;

    // Recalculate overall Veda progress
    UpdateVedaProgress(veda);

    await _progressRepo.UpdateAsync(progress);
}

private void UpdateVedaProgress(Veda veda)
{
    if (veda.Subtypes.Count == 0) return;

    var completed = veda.Subtypes.Count(s => s.Result.HasValue);
    var passed = veda.Subtypes.Count(s => s.Result == true);
    
    // Calculate progress based on passed subtypes
    veda.ProgressPercentage = (double)passed / veda.Subtypes.Count * 100;
}



// Updated MergeWithDefaultProgress method for Vedas
private Progress MergeWithDefaultProgress(Progress existingProgress)
{

            // Define all expected courses, vedas, and services
        var expectedCourses = new List<string>
        {
            "Vedic Studies",
            "Meditation Techniques",
            "Yoga Philosophy",
            "Spiritual Counseling",
            "Temple Rituals",
            "Energy Healing"
        };

        var expectedServices = new List<string>
        {
            "QuizMaster"
        };
    // Define expected Veda structure with subtypes
    var expectedVedas = new Dictionary<string, List<string>>
    {
        {"Natya", new List<string> {"NatyaQuiz"}},
        {"Ayurveda", new List<string> {"AyurvedaQuiz"}},
        {"Dhanurveda", new List<string>{"DhanurvedaQuiz"}},
        {"Shilpa", new List<string>{"ShilpaQuiz"}},
        {"Aranyakas", new List<string> {"Aitareya Aranyaka","Brihadaranyaka","Sankhyayana Aranyaka","Tittiri Aranyaka"}},
        {"Upveda" , new List<string>{"UpvedaQuiz"}  },
        {"Vedanga" , new List<string>{"Shiksha","Chandas","Vyakarana","Nirukta","Jyotisha","Kalpa"}},
        {"Brahmanas", new List<string>{"Rigveda","Samaveda","Yajurveda","Atharvaveda"}},
        {"Upanishad", new List<string>{"Aitareyopanishad","Arsheyopnishad","Brihadaranyakaupnishad","Chandogyopnishad","Ishavasayopanishad","Kathopnishad","Kenopnishad","Maitrayani","Mandukopnishad","Mundakopanishad","Prashnopanishad","Shetashvataropanishad","Taittiryopanishad"}}
        
        // Add other Vedas and their subtypes
    };

    var mergedVedas = expectedVedas.Select(kvp =>
    {
        var existing = existingProgress.Vedas.FirstOrDefault(v => v.VedaName == kvp.Key);
        if (existing == null)
        {
            existing = new Veda
            {
                VedaName = kvp.Key,
                ProgressPercentage = 0,
                LastAccessed = DateTime.UtcNow,
                Subtypes = kvp.Value.Select(s => new VedaSubtype
                {
                    SubtypeName = s,
                    Result = null,
                    LastAttempted = DateTime.MinValue
                }).ToList()
            };
        }
        else
        {
            // Merge expected subtypes
            foreach (var subtypeName in kvp.Value)
            {
                if (!existing.Subtypes.Any(s => s.SubtypeName == subtypeName))
                {
                    existing.Subtypes.Add(new VedaSubtype
                    {
                        SubtypeName = subtypeName,
                        Result = null,
                        LastAttempted = DateTime.MinValue
                    });
                }
            }
            UpdateVedaProgress(existing);
        }
        return existing;
    }).ToList();



    // Rest of the merge logic remains same...

        // Merge courses
        var mergedCourses = expectedCourses.Select(course =>
            existingProgress.Courses.FirstOrDefault(c => c.CourseName == course)
            ?? new CourseProgress
            {
                CourseName = course,
                ProgressPercentage = 0,
                LastAccessed = DateTime.UtcNow
            }).ToList();
// Update the MergeWithDefaultProgress method's services section
var mergedServices = expectedServices.Select(service =>
{
    var existingAttempts = existingProgress.Services
        .Where(s => s.ServiceName == service)
        .SelectMany(s => s.QuizAttempts)
        .ToList();

    return new ServiceUsage
    {
        ServiceName = service,
        QuizAttempts = existingAttempts,
        LastUsed = existingAttempts.Any() 
            ? existingAttempts.Max(a => a.CompletionTime) 
            : DateTime.UtcNow
    };
}).ToList();




    return new Progress
    {
        Id = existingProgress.Id,
        UserId = existingProgress.UserId,
        Courses = mergedCourses,
        Services = mergedServices,
        Vedas = mergedVedas,
        LastUpdated = existingProgress.LastUpdated
    };



}


    // In ProgressService.cs
private static readonly Dictionary<string, int> CourseDurations = new()
{
    {"Meditation Techniques", 600},  // 10 hours in minutes
    {"Vedic Studies", 1200},
    {"Yoga Philosophy", 900},
    {"Spiritual Counseling", 800},
    {"Temple Rituals", 700},
    {"Energy Healing", 1000}
};

// In ProgressService.cs
public async Task UpdateCourseProgress(string userId, string courseName, int minutesPracticed)
{
    if (!CourseDurations.TryGetValue(courseName, out var totalMinutes))
        throw new ArgumentException("Invalid course name");

    var progress = await GetUserProgress(userId);
    var course = progress.Courses.FirstOrDefault(c => c.CourseName == courseName);

    if (course == null) return;

    // Calculate percentage gained from this practice session
    var percentageGained = (minutesPracticed / (double)totalMinutes) * 100;
    
    // Update progress
    course.ProgressPercentage = Math.Min(
        course.ProgressPercentage + percentageGained, 
        100
    );
    course.LastAccessed = DateTime.UtcNow;

    // Update database
    await _progressRepo.UpdateAsync(progress);
}

public async Task RecordQuizAttempt(string userId, string serviceName, string topicName, double score)
{
    var progress = await GetUserProgress(userId);
    var service = progress.Services.FirstOrDefault(s => s.ServiceName == serviceName);

    if (service == null)
    {
        service = new ServiceUsage { ServiceName = serviceName };
        progress.Services.Add(service);
    }

    service.QuizAttempts.Add(new QuizAttempt
    {
        TopicName = topicName,
        Score = score,
        CompletionTime = DateTime.UtcNow
    });
    
    service.LastUsed = DateTime.UtcNow;
    await _progressRepo.UpdateAsync(progress);
}
}