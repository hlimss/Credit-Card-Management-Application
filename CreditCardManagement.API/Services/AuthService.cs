using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;
using BCrypt.Net;

namespace CreditCardManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // Check if it's a database connection issue
            if (ex.InnerException != null && 
                (ex.InnerException.Message.Contains("transient") || 
                 ex.InnerException.Message.Contains("connection") ||
                 ex.InnerException.Message.Contains("timeout")))
            {
                throw new InvalidOperationException("Database connection error. Please ensure SQL Server is running and try again.");
            }
            throw;
        }

        // Generate token
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes())
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes())
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetSecretKey());

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = GetJwtIssuer(),
                ValidAudience = GetJwtAudience(),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        return await Task.FromResult(GenerateJwtToken(user));
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(GetSecretKey());

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(GetJwtExpirationMinutes()),
            Issuer = GetJwtIssuer(),
            Audience = GetJwtAudience(),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GetSecretKey()
    {
        return _configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey is not configured");
    }

    private string GetJwtIssuer()
    {
        return _configuration["JwtSettings:Issuer"] ?? "CreditCardManagement";
    }

    private string GetJwtAudience()
    {
        return _configuration["JwtSettings:Audience"] ?? "CreditCardManagementUsers";
    }

    private int GetJwtExpirationMinutes()
    {
        return int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
    }
}

