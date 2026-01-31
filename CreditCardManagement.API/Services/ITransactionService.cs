using CreditCardManagement.API.DTOs;

namespace CreditCardManagement.API.Services;

public interface ITransactionService
{
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createDto, Guid userId);
    Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, Guid userId);
    Task<IEnumerable<TransactionDto>> GetTransactionsByCardIdAsync(Guid cardId, Guid userId);
    Task<IEnumerable<TransactionDto>> GetAllUserTransactionsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<CardExpensesDto> GetCardExpensesAsync(Guid cardId, Guid userId);
    Task<TransactionDto> UpdateTransactionAsync(Guid transactionId, UpdateTransactionDto updateDto, Guid userId);
    Task<bool> DeleteTransactionAsync(Guid transactionId, Guid userId);
    Task<Dictionary<string, object>> GetExpenseAnalyticsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
}
