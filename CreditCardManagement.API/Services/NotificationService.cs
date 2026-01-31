using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CheckAndSendExpirationAlertsAsync()
    {
        var cards = await _context.CreditCards
            .Include(c => c.User)
            .Where(c => c.IsActive)
            .ToListAsync();

        foreach (var card in cards)
        {
            var expirationInfo = CalculateExpirationInfo(card.ExpirationDate);
            
            if (expirationInfo.IsExpiringSoon && expirationInfo.DaysUntilExpiration <= 30)
            {
                await SendExpirationAlertAsync(card, expirationInfo.DaysUntilExpiration);
            }
        }
    }

    public async Task SendExpirationAlertAsync(CreditCard card, int daysUntilExpiration)
    {
        // In a real application, you would:
        // 1. Send email notification
        // 2. Send in-app notification
        // 3. Send webhook if configured
        // 4. Store notification in database
        
        _logger.LogInformation(
            "Expiration alert: Card {CardId} for user {UserId} expires in {Days} days",
            card.Id, card.UserId, daysUntilExpiration);

        // TODO: Implement actual notification sending
        // await _emailService.SendExpirationAlertAsync(card.User.Email, card, daysUntilExpiration);
        // await _notificationRepository.CreateAsync(new Notification { ... });
    }

    public async Task<List<ExpirationAlertDto>> GetExpiringCardsAsync(Guid userId)
    {
        var cards = await _context.CreditCards
            .Where(c => c.UserId == userId && c.IsActive)
            .ToListAsync();

        var alerts = new List<ExpirationAlertDto>();

        foreach (var card in cards)
        {
            var expirationInfo = CalculateExpirationInfo(card.ExpirationDate);
            
            if (expirationInfo.IsExpiringSoon && expirationInfo.DaysUntilExpiration <= 30)
            {
                alerts.Add(new ExpirationAlertDto
                {
                    CardId = card.Id,
                    CardholderName = card.CardholderName,
                    CardType = card.CardType,
                    ExpirationDate = card.ExpirationDate,
                    DaysUntilExpiration = expirationInfo.DaysUntilExpiration,
                    Category = card.Category
                });
            }
        }

        return alerts.OrderBy(a => a.DaysUntilExpiration).ToList();
    }

    private (int DaysUntilExpiration, bool IsExpiringSoon) CalculateExpirationInfo(string expirationDate)
    {
        try
        {
            var parts = expirationDate.Split('/');
            if (parts.Length != 2) return (0, false);

            var month = int.Parse(parts[0]);
            var year = int.Parse(parts[1]);
            var fullYear = 2000 + year;

            var expiration = new DateTime(fullYear, month, DateTime.DaysInMonth(fullYear, month));
            var now = DateTime.UtcNow;
            var daysUntilExpiration = (int)(expiration - now).TotalDays;
            var isExpiringSoon = daysUntilExpiration <= 30 && daysUntilExpiration >= 0;

            return (daysUntilExpiration, isExpiringSoon);
        }
        catch
        {
            return (0, false);
        }
    }
}
