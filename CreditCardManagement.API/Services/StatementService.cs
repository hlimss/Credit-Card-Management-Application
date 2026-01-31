using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public class StatementService : IStatementService
{
    private readonly ApplicationDbContext _context;

    public StatementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StatementDto> GenerateStatementAsync(CreateStatementDto createDto, Guid userId)
    {
        // Vérifier que la carte appartient à l'utilisateur
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == createDto.CreditCardId && c.UserId == userId);

        if (card == null)
        {
            throw new UnauthorizedAccessException("Carte non trouvée ou accès refusé");
        }

        // Déterminer les dates si non fournies
        var endDate = createDto.EndDate ?? DateTime.UtcNow;
        var startDate = createDto.StartDate ?? endDate.AddMonths(-1);

        // Récupérer toutes les transactions dans la période
        var transactions = await _context.Transactions
            .Where(t => t.CreditCardId == createDto.CreditCardId && 
                       t.TransactionDate >= startDate && 
                       t.TransactionDate <= endDate)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync();

        // Calculer les totaux
        var totalCredits = transactions
            .Where(t => t.TransactionType == "Credit" || t.TransactionType == "Refund")
            .Sum(t => t.Amount);

        var totalDebits = transactions
            .Where(t => t.TransactionType == "Expense")
            .Sum(t => t.Amount);

        // Calculer le solde d'ouverture (solde actuel + débits - crédits)
        var currentBalance = card.Balance ?? 0;
        var openingBalance = currentBalance + totalDebits - totalCredits;
        var closingBalance = currentBalance;

        var statement = new Statement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreditCardId = createDto.CreditCardId,
            StatementType = createDto.StatementType,
            StartDate = startDate,
            EndDate = endDate,
            OpeningBalance = openingBalance,
            ClosingBalance = closingBalance,
            TotalCredits = totalCredits,
            TotalDebits = totalDebits,
            TransactionCount = transactions.Count,
            GeneratedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Statements.Add(statement);
        await _context.SaveChangesAsync();

        return MapToDto(statement, card.CardholderName);
    }

    public async Task<IEnumerable<StatementDto>> GetUserStatementsAsync(Guid userId, Guid? creditCardId = null)
    {
        var query = _context.Statements
            .Include(s => s.CreditCard)
            .Where(s => s.UserId == userId);

        if (creditCardId.HasValue)
        {
            query = query.Where(s => s.CreditCardId == creditCardId.Value);
        }

        var statements = await query
            .OrderByDescending(s => s.EndDate)
            .ToListAsync();

        return statements.Select(s => MapToDto(s, s.CreditCard.CardholderName));
    }

    public async Task<StatementDto?> GetStatementByIdAsync(Guid statementId, Guid userId)
    {
        var statement = await _context.Statements
            .Include(s => s.CreditCard)
            .FirstOrDefaultAsync(s => s.Id == statementId && s.UserId == userId);

        return statement == null ? null : MapToDto(statement, statement.CreditCard.CardholderName);
    }

    private static StatementDto MapToDto(Statement statement, string cardName)
    {
        return new StatementDto
        {
            Id = statement.Id,
            CreditCardId = statement.CreditCardId,
            CreditCardName = cardName,
            StatementType = statement.StatementType,
            StartDate = statement.StartDate,
            EndDate = statement.EndDate,
            OpeningBalance = statement.OpeningBalance,
            ClosingBalance = statement.ClosingBalance,
            TotalCredits = statement.TotalCredits,
            TotalDebits = statement.TotalDebits,
            TransactionCount = statement.TransactionCount,
            Notes = statement.Notes,
            GeneratedAt = statement.GeneratedAt,
            CreatedAt = statement.CreatedAt
        };
    }
}
