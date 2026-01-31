using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public interface INotificationService
{
    Task CheckAndSendExpirationAlertsAsync();
    Task SendExpirationAlertAsync(CreditCard card, int daysUntilExpiration);
    Task<List<ExpirationAlertDto>> GetExpiringCardsAsync(Guid userId);
}

public class ExpirationAlertDto
{
    public Guid CardId { get; set; }
    public string CardholderName { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public int DaysUntilExpiration { get; set; }
    public string Category { get; set; } = string.Empty;
}
