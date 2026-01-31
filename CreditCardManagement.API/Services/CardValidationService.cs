using System.Text.RegularExpressions;

namespace CreditCardManagement.API.Services;

public class CardValidationService : ICardValidationService
{
    public bool ValidateCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        // Remove spaces and dashes
        var cleaned = Regex.Replace(cardNumber, @"[\s-]", "");
        
        // Check if all digits
        if (!Regex.IsMatch(cleaned, @"^\d+$"))
            return false;

        // Check length (13-19 digits)
        if (cleaned.Length < 13 || cleaned.Length > 19)
            return false;

        // Luhn algorithm validation
        return LuhnCheck(cleaned);
    }

    public bool ValidateExpirationDate(string expirationDate)
    {
        if (string.IsNullOrWhiteSpace(expirationDate))
            return false;

        // Check format MM/YY
        if (!Regex.IsMatch(expirationDate, @"^(0[1-9]|1[0-2])\/([0-9]{2})$"))
            return false;

        var parts = expirationDate.Split('/');
        var month = int.Parse(parts[0]);
        var year = int.Parse(parts[1]);

        // Convert YY to full year (assuming 20YY for years 00-99)
        var fullYear = 2000 + year;

        // Check if date is in the future
        var expiration = new DateTime(fullYear, month, 1).AddMonths(1).AddDays(-1);
        return expiration >= DateTime.Now.Date;
    }

    public string DetectCardType(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return "Unknown";

        var cleaned = Regex.Replace(cardNumber, @"[\s-]", "");

        // Visa: starts with 4
        if (Regex.IsMatch(cleaned, @"^4"))
            return "Visa";

        // MasterCard: starts with 51-55 or 2221-2720
        if (Regex.IsMatch(cleaned, @"^5[1-5]") || Regex.IsMatch(cleaned, @"^2[2-7]"))
            return "MasterCard";

        // American Express: starts with 34 or 37
        if (Regex.IsMatch(cleaned, @"^3[47]"))
            return "American Express";

        // Discover: starts with 6011, 65, or 644-649
        if (Regex.IsMatch(cleaned, @"^6011") || Regex.IsMatch(cleaned, @"^65") || Regex.IsMatch(cleaned, @"^64[4-9]"))
            return "Discover";

        return "Unknown";
    }

    public string MaskCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return string.Empty;

        var cleaned = Regex.Replace(cardNumber, @"[\s-]", "");
        
        if (cleaned.Length < 4)
            return cardNumber;

        // Show last 4 digits, mask the rest
        var lastFour = cleaned.Substring(cleaned.Length - 4);
        var masked = new string('*', cleaned.Length - 4);
        
        return $"{masked}{lastFour}";
    }

    private bool LuhnCheck(string cardNumber)
    {
        int sum = 0;
        bool alternate = false;

        // Process digits from right to left
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(cardNumber[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}

