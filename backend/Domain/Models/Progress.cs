using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Progress
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("UserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }

    [BsonElement("Courses")]
    public List<CourseProgress> Courses { get; set; } = new();

    [BsonElement("Services")]
    public List<ServiceUsage> Services { get; set; } = new();

    [BsonElement("Veda")]
    public List<Veda> Vedas { get; set; } = new();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("LastUpdated")]
    public DateTime LastUpdated { get; set; }
}
// Updated Veda class
public class Veda
{
    [BsonElement("VedaName")]
    public string VedaName { get; set; }

    [BsonElement("ProgressPercentage")]
    [BsonRepresentation(BsonType.Double)]
    public double ProgressPercentage { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("LastAccessed")]
    public DateTime LastAccessed { get; set; }

    [BsonElement("Subtypes")]
    public List<VedaSubtype> Subtypes { get; set; } = new();
}

public class VedaSubtype
{
    [BsonElement("SubtypeName")]
    public string SubtypeName { get; set; }

    [BsonElement("Result")]
    [BsonRepresentation(BsonType.Boolean)]
    public bool? Result { get; set; } // null = not attempted, true = pass, false = fail

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("LastAttempted")]
    public DateTime LastAttempted { get; set; }
}
public class CourseProgress
{
    [BsonElement("CourseName")]
    public string CourseName { get; set; }

    [BsonElement("ProgressPercentage")]
    [BsonRepresentation(BsonType.Double)]
    public double ProgressPercentage { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("LastAccessed")]
    public DateTime LastAccessed { get; set; }
}


public class ServiceUsage
{
    [BsonElement("ServiceName")]
    public string ServiceName { get; set; }

    [BsonElement("QuizAttempts")]
    public List<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("LastUsed")]
    public DateTime LastUsed { get; set; }
}

public class QuizAttempt
{
    [BsonElement("TopicName")]
    public string TopicName { get; set; }

    [BsonElement("Score")]
    [BsonRepresentation(BsonType.Double)]
    public double Score { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [BsonElement("CompletionTime")]
    public DateTime CompletionTime { get; set; }
}