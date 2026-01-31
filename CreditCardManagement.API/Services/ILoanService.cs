using CreditCardManagement.API.DTOs;

namespace CreditCardManagement.API.Services;

public interface ILoanService
{
    Task<LoanDto> CreateLoanAsync(CreateLoanDto createDto, Guid userId);
    Task<IEnumerable<LoanDto>> GetUserLoansAsync(Guid userId);
    Task<LoanDto?> GetLoanByIdAsync(Guid loanId, Guid userId);
    Task<LoanDto> MakePaymentAsync(Guid loanId, decimal amount, Guid userId);
    Task<bool> DeleteLoanAsync(Guid loanId, Guid userId);
}
