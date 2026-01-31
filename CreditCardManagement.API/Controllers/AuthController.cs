using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Services;
using CreditCardManagement.API.Data;

namespace CreditCardManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _context;

    public AuthController(IAuthService authService, ApplicationDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            // Handle database connection errors
            var errorMessage = "Database connection error. Please try again in a few moments.";
            if (ex.InnerException != null)
            {
                errorMessage = $"Database error: {ex.InnerException.Message}";
            }
            return StatusCode(503, new { message = errorMessage });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during registration. Please try again.", error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult Logout()
    {
        // With JWT, logout is handled client-side by removing the token
        // In a production app, you might want to implement token blacklisting
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (userId == null || email == null)
        {
            return Unauthorized();
        }

        // Get user details from database
        var user = await _context.Users.FindAsync(Guid.Parse(userId));
        
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}",
            PhoneNumber = user.PhoneNumber
        });
    }
}

