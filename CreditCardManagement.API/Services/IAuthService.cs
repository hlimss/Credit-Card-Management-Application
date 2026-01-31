using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> GenerateJwtTokenAsync(User user);
}

