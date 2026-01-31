namespace CreditCardManagement.API.Services;

public interface ICardValidationService
{
    bool ValidateCardNumber(string cardNumber);
    bool ValidateExpirationDate(string expirationDate);
    string DetectCardType(string cardNumber);
    string MaskCardNumber(string cardNumber);
}

