using CreditCardManagement.API.DTOs;

namespace CreditCardManagement.API.Services;

public interface IBankTransferService
{
    Task<BankTransferDto> CreateTransferAsync(CreateBankTransferDto createDto, Guid userId);
    Task<IEnumerable<BankTransferDto>> GetUserTransfersAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<BankTransferDto?> GetTransferByIdAsync(Guid transferId, Guid userId);
    Task<bool> CancelTransferAsync(Guid transferId, Guid userId);
}
