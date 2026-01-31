namespace CreditCardManagement.API.Services;

public interface IWhatsAppService
{
    Task<bool> SendCardAddedMessageAsync(string phoneNumber, string cardholderName, string cardType, decimal? balance);
    Task<bool> SendMessageAsync(string phoneNumber, string message);
}
