using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly ProgressService _progressService;

    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }
// In ProgressController.cs
[HttpPost("update")]
public async Task<IActionResult> UpdateProgress([FromBody] ProgressUpdateDto dto)
{
    try
    {
     //   var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             var userId = Request.Headers["X-User-Id"].ToString();
        Console.WriteLine($"UserId from header: {userId ?? "null"}");

        await _progressService.UpdateCourseProgress(userId, dto.CourseName, dto.MinutesPracticed);
        return Ok();
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserProgress(string userId)
    {
        var progress = await _progressService.GetUserProgress(userId);
        return Ok(progress);
    }

    [HttpGet("current-progress")]
    public async Task<IActionResult> GetCurrentUserProgress()
    {
        // Get UserId from header (X-User-Id)
        var userId = Request.Headers["X-User-Id"].ToString();
        Console.WriteLine($"UserId from header: {userId ?? "null"}");

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not provided in header." });
        }

        try
        {
            var progress = await _progressService.GetUserProgress(userId);
            return Ok(new
            {
                Courses = progress.Courses.ToDictionary(c => c.CourseName),
                Veda = progress.Vedas.ToDictionary(c=> c.VedaName),
                Services = progress.Services.ToDictionary(s => s.ServiceName)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("quiz-attempt")]
public async Task<IActionResult> RecordQuizAttempt([FromBody] QuizAttemptRequest request)
{
    try
    {
        var userId = Request.Headers["X-User-Id"].ToString();
        await _progressService.RecordQuizAttempt(userId, request.ServiceName, request.TopicName, request.Score);
        return Ok();
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}
[HttpPost("veda-subtype")]
public async Task<IActionResult> UpdateVedaSubtype([FromBody] VedaSubtypeUpdateRequest request)
{
    try
    {
        var userId = Request.Headers["X-User-Id"].ToString();
        await _progressService.UpdateVedaSubtype(userId, request.VedaName, request.SubtypeName, request.Result);
        return Ok();
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}



}

public class VedaSubtypeUpdateRequest
{
    public string VedaName { get; set; }
    public string SubtypeName { get; set; }
    public bool Result { get; set; }
}
public class QuizAttemptRequest
{
    public string? ServiceName { get; set; }
    public string? TopicName { get; set; }
    public double Score { get; set; }
}


public class CourseUpdateRequest
{
    public string CourseName { get; set; }
    public double Progress { get; set; }
}

public class ServiceUpdateRequest
{
    public string ServiceName { get; set; }
}

public class ProgressUpdateDto
{
    public string CourseName { get; set; }
    public int MinutesPracticed { get; set; }
}