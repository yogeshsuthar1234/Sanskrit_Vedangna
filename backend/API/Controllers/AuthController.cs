using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Domain.Models;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    
[HttpGet("user")]
public async Task<IActionResult> GetUser()
{
    try
    {
        var userId = Request.Headers["X-User-Id"].ToString();
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { 
                message = "Authorization required",
                solution = "Add X-User-Id header with valid user ID"
            });
        }

        var user = await _authService.GetUserById(userId);
        
        if (user == null)
        {
            return NotFound(new { 
                message = "User not found",
                userId = userId
            });
        }

        // Never return password in response
        return Ok(new {
            id = user.Id,
            username = user.Username,
            email = user.Email
        });
    }
    catch (FormatException)
    {
        return BadRequest(new { 
            message = "Invalid user ID format",
            requiredFormat = "24-character hexadecimal string"
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new {
            message = "Error retrieving user data",
            error = ex.Message
        });
    }
}
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        try
        {
            var result = await _authService.Register(user);
            return Created("", result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var user = await _authService.Login(dto.Username, dto.Password);
            // Return userId in response instead of using session
            return Ok(new { userId = user.Id });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // No session to clear, just return OK
        return Ok();
    }
}

public class LoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}