using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionDto> CreateTransactionAsync(CreateTransactionDto createDto, Guid userId)
    {
        // Verify that the card belongs to the user
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == createDto.CreditCardId && c.UserId == userId);

        if (card == null)
        {
            throw new UnauthorizedAccessException("Credit card not found or access denied");
        }

        // Si c'est une dépense (Expense), déduire le montant de la balance de la carte
        if (createDto.TransactionType == "Expense" && createDto.Amount > 0)
        {
            // Initialiser la balance à 0 si elle est null
            if (!card.Balance.HasValue)
            {
                card.Balance = 0;
            }

            // Vérifier que la balance est suffisante
            if (card.Balance.Value < createDto.Amount)
            {
                throw new InvalidOperationException($"Solde insuffisant. Solde actuel: {card.Balance.Value:C}, Montant requis: {createDto.Amount:C}");
            }

            // Déduire le montant de la balance
            card.Balance -= createDto.Amount;
            card.UpdatedAt = DateTime.UtcNow;
        }
        // Si c'est un crédit (Credit) ou un remboursement, ajouter le montant à la balance
        else if ((createDto.TransactionType == "Credit" || createDto.TransactionType == "Refund") && createDto.Amount > 0)
        {
            // Initialiser la balance à 0 si elle est null
            if (!card.Balance.HasValue)
            {
                card.Balance = 0;
            }

            // Ajouter le montant à la balance
            card.Balance += createDto.Amount;
            card.UpdatedAt = DateTime.UtcNow;
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            CreditCardId = createDto.CreditCardId,
            MerchantName = createDto.MerchantName,
            Amount = createDto.Amount,
            Currency = createDto.Currency,
            Category = createDto.Category,
            Description = createDto.Description,
            TransactionDate = createDto.TransactionDate,
            TransactionType = createDto.TransactionType,
            Location = createDto.Location,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return MapToDto(transaction);
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(Guid transactionId, Guid userId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.CreditCard)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.CreditCard.UserId == userId);

        return transaction == null ? null : MapToDto(transaction);
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionsByCardIdAsync(Guid cardId, Guid userId)
    {
        var transactions = await _context.Transactions
            .Include(t => t.CreditCard)
            .Where(t => t.CreditCardId == cardId && t.CreditCard.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        return transactions.Select(MapToDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetAllUserTransactionsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Include(t => t.CreditCard)
            .Where(t => t.CreditCard.UserId == userId);

        if (startDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= endDate.Value);
        }

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        return transactions.Select(MapToDto);
    }

    public async Task<CardExpensesDto> GetCardExpensesAsync(Guid cardId, Guid userId)
    {
        var card = await _context.CreditCards
            .FirstOrDefaultAsync(c => c.Id == cardId && c.UserId == userId);

        if (card == null)
        {
            throw new UnauthorizedAccessException("Credit card not found or access denied");
        }

        var transactions = await _context.Transactions
            .Where(t => t.CreditCardId == cardId)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var thisMonthStart = new DateTime(now.Year, now.Month, 1);
        var lastMonthStart = thisMonthStart.AddMonths(-1);
        var lastMonthEnd = thisMonthStart.AddDays(-1);

        var totalAmount = transactions.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount);
        var totalTransactions = transactions.Count;
        var averageAmount = totalTransactions > 0 ? totalAmount / totalTransactions : 0;

        var thisMonthAmount = transactions
            .Where(t => t.TransactionType == "Expense" && t.TransactionDate >= thisMonthStart)
            .Sum(t => t.Amount);

        var lastMonthAmount = transactions
            .Where(t => t.TransactionType == "Expense" && t.TransactionDate >= lastMonthStart && t.TransactionDate <= lastMonthEnd)
            .Sum(t => t.Amount);

        var categoryBreakdown = transactions
            .Where(t => t.TransactionType == "Expense")
            .GroupBy(t => t.Category)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));

        // Get last 4 digits of card (decrypt if needed, for now just show placeholder)
        var last4Digits = "****";

        var recentTransactions = transactions
            .OrderByDescending(t => t.TransactionDate)
            .Take(10)
            .Select(MapToDto)
            .ToList();

        return new CardExpensesDto
        {
            CardId = cardId,
            CardholderName = card.CardholderName,
            CardType = card.CardType,
            Last4Digits = last4Digits,
            TotalTransactions = totalTransactions,
            TotalAmount = totalAmount,
            AverageAmount = averageAmount,
            ThisMonthAmount = thisMonthAmount,
            LastMonthAmount = lastMonthAmount > 0 ? lastMonthAmount : null,
            RecentTransactions = recentTransactions,
            CategoryBreakdown = categoryBreakdown
        };
    }

    public async Task<TransactionDto> UpdateTransactionAsync(Guid transactionId, UpdateTransactionDto updateDto, Guid userId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.CreditCard)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.CreditCard.UserId == userId);

        if (transaction == null)
        {
            throw new UnauthorizedAccessException("Transaction not found or access denied");
        }

        var card = transaction.CreditCard;
        var oldAmount = transaction.Amount;
        var oldType = transaction.TransactionType;

        // Restaurer l'ancien montant dans la balance avant d'appliquer les modifications
        if (oldType == "Expense" && oldAmount > 0)
        {
            if (!card.Balance.HasValue)
                card.Balance = 0;
            card.Balance += oldAmount; // Restaurer le montant déduit
        }
        else if ((oldType == "Credit" || oldType == "Refund") && oldAmount > 0)
        {
            if (!card.Balance.HasValue)
                card.Balance = 0;
            card.Balance -= oldAmount; // Retirer le montant ajouté
        }

        // Appliquer les modifications
        if (!string.IsNullOrEmpty(updateDto.MerchantName))
            transaction.MerchantName = updateDto.MerchantName;
        if (updateDto.Amount.HasValue)
            transaction.Amount = updateDto.Amount.Value;
        if (!string.IsNullOrEmpty(updateDto.Currency))
            transaction.Currency = updateDto.Currency;
        if (!string.IsNullOrEmpty(updateDto.Category))
            transaction.Category = updateDto.Category;
        if (updateDto.Description != null)
            transaction.Description = updateDto.Description;
        if (updateDto.TransactionDate.HasValue)
            transaction.TransactionDate = updateDto.TransactionDate.Value;
        if (!string.IsNullOrEmpty(updateDto.TransactionType))
            transaction.TransactionType = updateDto.TransactionType;
        if (updateDto.Location != null)
            transaction.Location = updateDto.Location;

        // Appliquer le nouveau montant à la balance
        var newAmount = updateDto.Amount ?? transaction.Amount;
        var newType = updateDto.TransactionType ?? transaction.TransactionType;

        if (newType == "Expense" && newAmount > 0)
        {
            if (!card.Balance.HasValue)
                card.Balance = 0;
            
            // Vérifier que la balance est suffisante
            if (card.Balance.Value < newAmount)
            {
                throw new InvalidOperationException($"Solde insuffisant. Solde actuel: {card.Balance.Value:C}, Montant requis: {newAmount:C}");
            }
            
            card.Balance -= newAmount; // Déduire le nouveau montant
        }
        else if ((newType == "Credit" || newType == "Refund") && newAmount > 0)
        {
            if (!card.Balance.HasValue)
                card.Balance = 0;
            card.Balance += newAmount; // Ajouter le nouveau montant
        }

        card.UpdatedAt = DateTime.UtcNow;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(transaction);
    }

    public async Task<bool> DeleteTransactionAsync(Guid transactionId, Guid userId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.CreditCard)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.CreditCard.UserId == userId);

        if (transaction == null)
        {
            return false;
        }

        var card = transaction.CreditCard;

        // Restaurer le montant dans la balance de la carte
        if (transaction.TransactionType == "Expense" && transaction.Amount > 0)
        {
            // Si c'était une dépense, remettre le montant dans la balance
            if (!card.Balance.HasValue)
                card.Balance = 0;
            card.Balance += transaction.Amount;
            card.UpdatedAt = DateTime.UtcNow;
        }
        else if ((transaction.TransactionType == "Credit" || transaction.TransactionType == "Refund") && transaction.Amount > 0)
        {
            // Si c'était un crédit, retirer le montant de la balance
            if (!card.Balance.HasValue)
                card.Balance = 0;
            card.Balance -= transaction.Amount;
            // S'assurer que la balance ne devient pas négative
            if (card.Balance < 0)
                card.Balance = 0;
            card.UpdatedAt = DateTime.UtcNow;
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Dictionary<string, object>> GetExpenseAnalyticsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Transactions
            .Include(t => t.CreditCard)
            .Where(t => t.CreditCard.UserId == userId && t.TransactionType == "Expense");

        if (startDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= endDate.Value);
        }

        var transactions = await query.ToListAsync();

        var analytics = new Dictionary<string, object>
        {
            ["totalExpenses"] = transactions.Sum(t => t.Amount),
            ["totalTransactions"] = transactions.Count,
            ["averageTransaction"] = transactions.Count > 0 ? transactions.Average(t => t.Amount) : 0,
            ["categoryBreakdown"] = transactions
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
            ["dailyExpenses"] = transactions
                .GroupBy(t => t.TransactionDate.Date)
                .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Sum(t => t.Amount)),
            ["hourlyExpenses"] = transactions
                .GroupBy(t => t.TransactionDate.Hour)
                .ToDictionary(g => g.Key.ToString(), g => g.Sum(t => t.Amount)),
            ["monthlyExpenses"] = transactions
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:D2}", g => g.Sum(t => t.Amount))
        };

        return analytics;
    }

    private static TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            CreditCardId = transaction.CreditCardId,
            MerchantName = transaction.MerchantName,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Category = transaction.Category,
            Description = transaction.Description,
            TransactionDate = transaction.TransactionDate,
            TransactionType = transaction.TransactionType,
            Location = transaction.Location,
            CreatedAt = transaction.CreatedAt
        };
    }
}
