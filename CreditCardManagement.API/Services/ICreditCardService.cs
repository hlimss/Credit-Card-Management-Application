using CreditCardManagement.API.DTOs;

namespace CreditCardManagement.API.Services;

public interface ICreditCardService
{
    Task<IEnumerable<CreditCardDto>> GetUserCreditCardsAsync(Guid userId);
    Task<CreditCardDto?> GetCreditCardByIdAsync(Guid id, Guid userId);
    Task<CreditCardDto> CreateCreditCardAsync(CreateCreditCardDto createDto, Guid userId);
    Task<CreditCardDto> UpdateCreditCardAsync(Guid id, UpdateCreditCardDto updateDto, Guid userId);
    Task<bool> DeleteCreditCardAsync(Guid id, Guid userId);
}

