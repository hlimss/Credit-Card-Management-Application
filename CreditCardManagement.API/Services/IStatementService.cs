using CreditCardManagement.API.DTOs;

namespace CreditCardManagement.API.Services;

public interface IStatementService
{
    Task<StatementDto> GenerateStatementAsync(CreateStatementDto createDto, Guid userId);
    Task<IEnumerable<StatementDto>> GetUserStatementsAsync(Guid userId, Guid? creditCardId = null);
    Task<StatementDto?> GetStatementByIdAsync(Guid statementId, Guid userId);
}
