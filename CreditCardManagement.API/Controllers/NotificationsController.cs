using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CreditCardManagement.API.Services;

namespace CreditCardManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("expiring-cards")]
    public async Task<ActionResult> GetExpiringCards()
    {
        try
        {
            var userId = GetCurrentUserId();
            var alerts = await _notificationService.GetExpiringCardsAsync(userId);
            return Ok(alerts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving expiration alerts", error = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }
}
