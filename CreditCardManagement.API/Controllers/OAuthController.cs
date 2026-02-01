using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.Models;
using CreditCardManagement.API.Services;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace CreditCardManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OAuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public OAuthController(ApplicationDbContext context, IAuthService authService, IConfiguration configuration, IAuthenticationSchemeProvider schemeProvider)
    {
        _context = context;
        _authService = authService;
        _configuration = configuration;
        _schemeProvider = schemeProvider;
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<OAuthController>>();
        
        // Configurer la redirection après l'authentification OAuth
        // On redirige vers notre endpoint backend qui générera le JWT
        var backendUrl = $"{Request.Scheme}://{Request.Host}";
        var properties = new AuthenticationProperties
        {
            RedirectUri = $"{backendUrl}/api/OAuth/google-callback-success"
        };
        
        logger.LogInformation("Initiating Google OAuth login");
        
        return Challenge(properties, "Google");
    }

    [HttpGet("facebook")]
    public IActionResult FacebookLogin()
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<OAuthController>>();
        
        // Configurer la redirection après l'authentification OAuth
        var backendUrl = $"{Request.Scheme}://{Request.Host}";
        var properties = new AuthenticationProperties
        {
            RedirectUri = $"{backendUrl}/api/OAuth/facebook-callback-success"
        };
        
        logger.LogInformation("Initiating Facebook OAuth login");
        
        return Challenge(properties, "Facebook");
    }

    // Endpoint pour intercepter /signin-google après que le cookie soit écrit
    // Le middleware OAuth redirige vers /signin-google après avoir écrit le cookie
    // On intercepte cette requête pour lire le cookie et rediriger vers notre endpoint
    [HttpGet("signin-google")]
    public async Task<IActionResult> SignInGoogle()
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<OAuthController>>();
        logger.LogInformation("SignInGoogle endpoint intercepted - checking for cookie");
        
        // Essayer d'authentifier avec le cookie scheme
        var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        logger.LogInformation("Cookie authentication result: {Succeeded}, HasPrincipal: {HasPrincipal}", 
            cookieResult.Succeeded, cookieResult.Principal != null);
        
        if (cookieResult.Succeeded && cookieResult.Principal != null)
        {
            // Le cookie est disponible, rediriger vers notre endpoint de traitement
            logger.LogInformation("Cookie found, redirecting to google-callback-success");
            return Redirect("/api/OAuth/google-callback-success");
        }
        
        // Si le cookie n'est pas disponible, attendre un peu et réessayer
        logger.LogWarning("Cookie not found yet, waiting and retrying...");
        await Task.Delay(300);
        
        cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (cookieResult.Succeeded && cookieResult.Principal != null)
        {
            logger.LogInformation("Cookie found on retry, redirecting to google-callback-success");
            return Redirect("/api/OAuth/google-callback-success");
        }
        
        // Si toujours pas de cookie, rediriger vers le login avec une erreur
        logger.LogError("Cookie not found after retry");
        return Redirect($"{GetFrontendUrl()}/login?error=google_auth_failed");
    }

    [HttpGet("signin-facebook")]
    public async Task<IActionResult> SignInFacebook()
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<OAuthController>>();
        logger.LogInformation("SignInFacebook endpoint intercepted - checking for cookie");
        
        // Essayer d'authentifier avec le cookie scheme
        var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        logger.LogInformation("Cookie authentication result: {Succeeded}, HasPrincipal: {HasPrincipal}", 
            cookieResult.Succeeded, cookieResult.Principal != null);
        
        if (cookieResult.Succeeded && cookieResult.Principal != null)
        {
            // Le cookie est disponible, rediriger vers notre endpoint de traitement
            logger.LogInformation("Cookie found, redirecting to facebook-callback-success");
            return Redirect("/api/OAuth/facebook-callback-success");
        }
        
        // Si le cookie n'est pas disponible, attendre un peu et réessayer
        logger.LogWarning("Cookie not found yet, waiting and retrying...");
        await Task.Delay(300);
        
        cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (cookieResult.Succeeded && cookieResult.Principal != null)
        {
            logger.LogInformation("Cookie found on retry, redirecting to facebook-callback-success");
            return Redirect("/api/OAuth/facebook-callback-success");
        }
        
        // Si toujours pas de cookie, rediriger vers le login avec une erreur
        logger.LogError("Cookie not found after retry");
        return Redirect($"{GetFrontendUrl()}/login?error=facebook_auth_failed");
    }

    [HttpGet("google-callback-success")]
    public async Task<IActionResult> GoogleCallbackSuccess()
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<OAuthController>>();
        logger.LogInformation("Google callback success endpoint called");
        
        // Attendre un peu pour s'assurer que le cookie est disponible
        await Task.Delay(100);
        
        // Essayer d'authentifier avec le cookie scheme d'abord
        var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        logger.LogInformation("Cookie authentication result: {Succeeded}, HasPrincipal: {HasPrincipal}", 
            cookieResult.Succeeded, cookieResult.Principal != null);
        
        if (cookieResult.Succeeded && cookieResult.Principal != null)
        {
            logger.LogInformation("Cookie authentication succeeded");
            // Les claims Google sont dans le cookie
            var claims = cookieResult.Principal.Claims.ToList();
            logger.LogInformation("Number of claims: {Count}", claims.Count);
            
            // Log all claim types for debugging
            foreach (var claim in claims)
            {
                logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
            
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            logger.LogInformation("Email: {Email}, Name: {Name}, ProviderId: {ProviderId}", email ?? "null", name ?? "null", providerId ?? "null");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerId))
            {
                logger.LogWarning("Missing email or providerId from cookie claims");
                return Redirect($"{GetFrontendUrl()}/login?error=invalid_google_data");
            }

            var user = await HandleOAuthUser(email, name, "Google", providerId);
            var token = await _authService.GenerateJwtTokenAsync(user);

            // Encode URL parameters to handle special characters
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(email);
            var encodedFirstName = Uri.EscapeDataString(user.FirstName ?? "");
            var encodedLastName = Uri.EscapeDataString(user.LastName ?? "");

            var redirectUrl = $"{GetFrontendUrl()}/oauth-callback?token={encodedToken}&email={encodedEmail}&firstName={encodedFirstName}&lastName={encodedLastName}";
            
            logger.LogInformation("Redirecting to frontend: {RedirectUrl}", redirectUrl);
            
            return Redirect(redirectUrl);
        }
        
        // Si le cookie n'a pas fonctionné, essayer directement avec Google
        logger.LogWarning("Cookie authentication failed, trying Google scheme");
        
        var googleResult = await HttpContext.AuthenticateAsync("Google");
        logger.LogInformation("Google authentication result: {Succeeded}", googleResult.Succeeded);
        
        if (!googleResult.Succeeded)
        {
            logger.LogError("Google authentication failed: {Error}", googleResult.Failure?.Message);
            return Redirect($"{GetFrontendUrl()}/login?error=google_auth_failed");
        }

        var googleClaims = googleResult.Principal?.Claims.ToList();
        var googleEmail = googleClaims?.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
        var googleName = googleClaims?.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value;
        var googleProviderId = googleClaims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(googleEmail) || string.IsNullOrEmpty(googleProviderId))
        {
            logger.LogWarning("Missing email or providerId from Google claims");
            return Redirect($"{GetFrontendUrl()}/login?error=invalid_google_data");
        }

        var googleUser = await HandleOAuthUser(googleEmail, googleName, "Google", googleProviderId);
        var googleToken = await _authService.GenerateJwtTokenAsync(googleUser);

        var encodedGoogleToken = Uri.EscapeDataString(googleToken);
        var encodedGoogleEmail = Uri.EscapeDataString(googleEmail);
        var encodedGoogleFirstName = Uri.EscapeDataString(googleUser.FirstName ?? "");
        var encodedGoogleLastName = Uri.EscapeDataString(googleUser.LastName ?? "");

        var googleRedirectUrl = $"{GetFrontendUrl()}/oauth-callback?token={encodedGoogleToken}&email={encodedGoogleEmail}&firstName={encodedGoogleFirstName}&lastName={encodedGoogleLastName}";
        
        logger.LogInformation("Redirecting to frontend (Google scheme): {RedirectUrl}", googleRedirectUrl);
        
        return Redirect(googleRedirectUrl);
    }

    [HttpGet("facebook-callback-success")]
    public async Task<IActionResult> FacebookCallbackSuccess()
    {
        var logger = HttpContext.RequestServices.GetRequiredService<ILogger<OAuthController>>();
        logger.LogInformation("Facebook callback success endpoint called");
        
        // Essayer d'authentifier avec le cookie scheme d'abord
        var cookieResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (cookieResult.Succeeded)
        {
            logger.LogInformation("Cookie authentication succeeded");
            var claims = cookieResult.Principal?.Claims.ToList();
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value;
            var providerId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "id")?.Value;

            logger.LogInformation("Email: {Email}, Name: {Name}, ProviderId: {ProviderId}", email ?? "null", name ?? "null", providerId ?? "null");

            // Si email n'est pas disponible, utiliser un email basé sur le providerId
            if (string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(providerId))
            {
                email = $"{providerId}@facebook.temp";
            }

            if (string.IsNullOrEmpty(providerId))
            {
                logger.LogWarning("Missing providerId from cookie claims");
                return Redirect($"{GetFrontendUrl()}/login?error=invalid_facebook_data");
            }

            var user = await HandleOAuthUser(email, name, "Facebook", providerId);
            var token = await _authService.GenerateJwtTokenAsync(user);

            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(email ?? "");
            var encodedFirstName = Uri.EscapeDataString(user.FirstName ?? "");
            var encodedLastName = Uri.EscapeDataString(user.LastName ?? "");

            var redirectUrl = $"{GetFrontendUrl()}/oauth-callback?token={encodedToken}&email={encodedEmail}&firstName={encodedFirstName}&lastName={encodedLastName}";
            
            logger.LogInformation("Redirecting to frontend: {RedirectUrl}", redirectUrl);
            
            return Redirect(redirectUrl);
        }
        
        // Si le cookie n'a pas fonctionné, essayer directement avec Facebook
        logger.LogWarning("Cookie authentication failed, trying Facebook scheme");
        
        var facebookResult = await HttpContext.AuthenticateAsync("Facebook");
        if (!facebookResult.Succeeded)
        {
            logger.LogError("Facebook authentication failed: {Error}", facebookResult.Failure?.Message);
            return Redirect($"{GetFrontendUrl()}/login?error=facebook_auth_failed");
        }

        var facebookClaims = facebookResult.Principal?.Claims.ToList();
        var facebookEmail = facebookClaims?.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email")?.Value;
        var facebookName = facebookClaims?.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "name")?.Value;
        var facebookProviderId = facebookClaims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "id")?.Value;

        // Si email n'est pas disponible, utiliser un email basé sur le providerId
        if (string.IsNullOrEmpty(facebookEmail) && !string.IsNullOrEmpty(facebookProviderId))
        {
            facebookEmail = $"{facebookProviderId}@facebook.temp";
        }

        if (string.IsNullOrEmpty(facebookProviderId))
        {
            logger.LogWarning("Missing providerId from Facebook claims");
            return Redirect($"{GetFrontendUrl()}/login?error=invalid_facebook_data");
        }

        var facebookUser = await HandleOAuthUser(facebookEmail, facebookName, "Facebook", facebookProviderId);
        var facebookToken = await _authService.GenerateJwtTokenAsync(facebookUser);

        var encodedFacebookToken = Uri.EscapeDataString(facebookToken);
        var encodedFacebookEmail = Uri.EscapeDataString(facebookEmail ?? "");
        var encodedFacebookFirstName = Uri.EscapeDataString(facebookUser.FirstName ?? "");
        var encodedFacebookLastName = Uri.EscapeDataString(facebookUser.LastName ?? "");

        var facebookRedirectUrl = $"{GetFrontendUrl()}/oauth-callback?token={encodedFacebookToken}&email={encodedFacebookEmail}&firstName={encodedFacebookFirstName}&lastName={encodedFacebookLastName}";
        
        logger.LogInformation("Redirecting to frontend (Facebook scheme): {RedirectUrl}", facebookRedirectUrl);
        
        return Redirect(facebookRedirectUrl);
    }

    private async Task<User> HandleOAuthUser(string? email, string? name, string provider, string providerId)
    {
        // S'assurer que l'email n'est pas null
        if (string.IsNullOrEmpty(email))
        {
            email = $"{providerId}@facebook.temp";
        }
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email || (u.Provider == provider && u.ProviderId == providerId));

        if (user == null)
        {
            // Parse name into first and last name
            var nameParts = name?.Split(' ') ?? new[] { "User" };
            var firstName = nameParts[0];
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "User";

            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email, // email n'est plus null ici
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Random password for OAuth users
                Provider = provider,
                ProviderId = providerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else if (user.Provider != provider || user.ProviderId != providerId)
        {
            // Update provider info if different
            user.Provider = provider;
            user.ProviderId = providerId;
            await _context.SaveChangesAsync();
        }

        return user;
    }

    private string GetFrontendUrl()
    {
        return _configuration["FrontendUrl"] ?? "http://localhost:5173";
    }

    [HttpGet("test-redirect-uri")]
    public IActionResult TestRedirectUri()
    {
        var scheme = _configuration["OAuth:ForceHttp"] == "true" ? "http" : Request.Scheme;
        var redirectUrl = $"{scheme}://{Request.Host}/signin-google";
        
        return Ok(new
        {
            RedirectUri = redirectUrl,
            RequestScheme = Request.Scheme,
            RequestHost = Request.Host.ToString(),
            RequestProtocol = Request.Protocol,
            ConfigurationForceHttp = _configuration["OAuth:ForceHttp"],
            Message = "Add this exact URI to Google Cloud Console: " + redirectUrl,
            Instructions = new
            {
                Step1 = "Go to Google Cloud Console",
                Step2 = "Open your OAuth 2.0 Client ID",
                Step3 = "In 'Authorized redirect URIs', add: " + redirectUrl,
                Step4 = "Save and wait 1-2 minutes for changes to propagate"
            }
        });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetOAuthStatus()
    {
        var googleScheme = await _schemeProvider.GetSchemeAsync("Google");
        var facebookScheme = await _schemeProvider.GetSchemeAsync("Facebook");
        
        return Ok(new
        {
            google = googleScheme != null,
            facebook = facebookScheme != null
        });
    }
}
