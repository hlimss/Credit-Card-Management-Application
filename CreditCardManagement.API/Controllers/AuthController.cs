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

    [HttpPut("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (userId == null || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(userGuid);
            
            if (user == null)
            {
                return Unauthorized();
            }

            // Update user fields if provided
            if (!string.IsNullOrEmpty(updateDto.FirstName))
                user.FirstName = updateDto.FirstName;
            
            if (!string.IsNullOrEmpty(updateDto.LastName))
                user.LastName = updateDto.LastName;
            
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                // Check if email is already taken by another user
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == updateDto.Email && u.Id != userGuid);
                
                if (emailExists)
                {
                    return BadRequest(new { message = "This email is already in use by another account" });
                }
                
                user.Email = updateDto.Email;
            }
            
            if (updateDto.PhoneNumber != null)
                user.PhoneNumber = updateDto.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                PhoneNumber = user.PhoneNumber,
                message = "User information updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating user information", error = ex.Message });
        }
    }
}

