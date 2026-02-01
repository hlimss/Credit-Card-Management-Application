using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;
using System.Security.Cryptography;
using System.Text;

namespace CreditCardManagement.API.Services;

public class CreditCardService : ICreditCardService
{
    private readonly ApplicationDbContext _context;
    private readonly ICardValidationService _validationService;

    public CreditCardService(ApplicationDbContext context, ICardValidationService validationService)
    {
        _context = context;
        _validationService = validationService;
    }

    public async Task<IEnumerable<CreditCardDto>> GetUserCreditCardsAsync(Guid userId)
    {
        var cards = await _context.CreditCards
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return cards.Select(c => MapToDto(c));
    }

    public async Task<CreditCardDto?> GetCreditCardByIdAsync(Guid id, Guid userId)
    {
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (card == null)
            return null;

        return MapToDto(card);
    }

    public async Task<CreditCardDto> CreateCreditCardAsync(CreateCreditCardDto createDto, Guid userId)
    {
        // Validate card number
        if (string.IsNullOrWhiteSpace(createDto.CardNumber))
        {
            throw new ArgumentException("Card number is required");
        }

        var cleanedCardNumber = createDto.CardNumber.Replace(" ", "").Replace("-", "");
        if (!_validationService.ValidateCardNumber(cleanedCardNumber))
        {
            throw new ArgumentException("Invalid card number. The card number does not pass the Luhn algorithm validation. Please use a valid test card number (see TEST_CARD_NUMBERS.md for valid test numbers like 4111 1111 1111 1111 or 5555 5555 5555 4444).");
        }

        // Validate expiration date
        if (string.IsNullOrWhiteSpace(createDto.ExpirationDate))
        {
            throw new ArgumentException("Expiration date is required");
        }

        if (!_validationService.ValidateExpirationDate(createDto.ExpirationDate))
        {
            throw new ArgumentException("Invalid or expired expiration date. Please use MM/YY format and ensure the date is in the future.");
        }

        // Detect card type
        var cardType = _validationService.DetectCardType(createDto.CardNumber);

        var card = new CreditCard
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CardNumber = Encrypt(cleanedCardNumber),
            CardholderName = createDto.CardholderName,
            ExpirationDate = createDto.ExpirationDate,
            CVV = Encrypt(createDto.CVV),
            CardType = cardType,
            Category = createDto.Category ?? "Personnel",
            Tags = createDto.Tags,
            Balance = createDto.Balance,
            ConfirmationCode = createDto.ConfirmationCode,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            _context.CreditCards.Add(card);
            await _context.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // Check if it's a string length issue
            if (ex.InnerException?.Message?.Contains("String or binary data would be truncated") == true)
            {
                throw new InvalidOperationException("The encrypted card data is too long for the database field. Please contact support.", ex);
            }
            throw new InvalidOperationException($"Failed to save credit card to database: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save credit card to database: {ex.Message}", ex);
        }

        return MapToDto(card);
    }

    public async Task<CreditCardDto> UpdateCreditCardAsync(Guid id, UpdateCreditCardDto updateDto, Guid userId)
    {
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (card == null)
            throw new KeyNotFoundException("Credit card not found");

        // Validate card number only if it's being updated
        if (!string.IsNullOrEmpty(updateDto.CardNumber))
        {
            var cleanedCardNumber = updateDto.CardNumber.Replace(" ", "").Replace("-", "");
            if (!_validationService.ValidateCardNumber(cleanedCardNumber))
            {
                throw new ArgumentException("Invalid card number. The card number does not pass the Luhn algorithm validation.");
            }
            card.CardNumber = Encrypt(cleanedCardNumber);
            card.CardType = _validationService.DetectCardType(cleanedCardNumber);
        }

        // Validate expiration date only if it's being updated
        if (!string.IsNullOrEmpty(updateDto.ExpirationDate))
        {
            if (!_validationService.ValidateExpirationDate(updateDto.ExpirationDate))
            {
                throw new ArgumentException("Invalid or expired expiration date");
            }
            card.ExpirationDate = updateDto.ExpirationDate;
        }
        if (!string.IsNullOrEmpty(updateDto.CardholderName))
            card.CardholderName = updateDto.CardholderName;
        if (!string.IsNullOrEmpty(updateDto.CVV))
            card.CVV = Encrypt(updateDto.CVV);
        if (!string.IsNullOrEmpty(updateDto.Category))
            card.Category = updateDto.Category;
        if (updateDto.Tags != null)
            card.Tags = updateDto.Tags;
        if (updateDto.Balance.HasValue)
            card.Balance = updateDto.Balance.Value;
        if (!string.IsNullOrEmpty(updateDto.ConfirmationCode))
            card.ConfirmationCode = updateDto.ConfirmationCode;
        if (updateDto.IsActive.HasValue)
            card.IsActive = updateDto.IsActive.Value;
        card.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(card);
    }

    public async Task<bool> DeleteCreditCardAsync(Guid id, Guid userId)
    {
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (card == null)
            return false;

        _context.CreditCards.Remove(card);
        await _context.SaveChangesAsync();

        return true;
    }

    private CreditCardDto MapToDto(CreditCard card)
    {
        // Calculate days until expiration
        var expirationInfo = CalculateExpirationInfo(card.ExpirationDate);
        
        return new CreditCardDto
        {
            Id = card.Id,
            CardNumber = _validationService.MaskCardNumber(Decrypt(card.CardNumber)),
            CardholderName = card.CardholderName,
            ExpirationDate = card.ExpirationDate,
            CVV = "***", // Never expose CVV
            CardType = card.CardType,
            Category = card.Category,
            Tags = card.Tags,
            Balance = card.Balance,
            ConfirmationCode = card.ConfirmationCode,
            IsActive = card.IsActive,
            DaysUntilExpiration = expirationInfo.DaysUntilExpiration,
            IsExpiringSoon = expirationInfo.IsExpiringSoon,
            CreatedAt = card.CreatedAt,
            UpdatedAt = card.UpdatedAt
        };
    }

    private (int DaysUntilExpiration, bool IsExpiringSoon) CalculateExpirationInfo(string expirationDate)
    {
        try
        {
            var parts = expirationDate.Split('/');
            if (parts.Length != 2) return (0, false);

            var month = int.Parse(parts[0]);
            var year = int.Parse(parts[1]);
            var fullYear = 2000 + year; // Convert YY to YYYY

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

    // Simple encryption/decryption (in production, use proper encryption)
    private string Encrypt(string plainText)
    {
        // In production, use proper encryption like AES
        // This is a simplified version for demonstration
        var bytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(bytes);
    }

    private string Decrypt(string cipherText)
    {
        try
        {
            var bytes = Convert.FromBase64String(cipherText);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}

