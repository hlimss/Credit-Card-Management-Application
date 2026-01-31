using System.ComponentModel.DataAnnotations;

namespace CreditCardManagement.API.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid CreditCardId { get; set; }
    public string MerchantName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Category { get; set; } = "Other";
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = "Expense";
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTransactionDto
{
    [Required(ErrorMessage = "Credit card ID is required")]
    public Guid CreditCardId { get; set; }

    [Required(ErrorMessage = "Merchant name is required")]
    [StringLength(200, ErrorMessage = "Merchant name cannot exceed 200 characters")]
    public string MerchantName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999999.99")]
    public decimal Amount { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "USD";

    [StringLength(50)]
    public string Category { get; set; } = "Other";

    [StringLength(500)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Transaction date is required")]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string TransactionType { get; set; } = "Expense";

    [StringLength(100)]
    public string? Location { get; set; }
}

public class UpdateTransactionDto
{
    [StringLength(200)]
    public string? MerchantName { get; set; }

    [Range(0.01, 999999.99)]
    public decimal? Amount { get; set; }

    [StringLength(3)]
    public string? Currency { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? TransactionDate { get; set; }

    [StringLength(50)]
    public string? TransactionType { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }
}

public class CardExpensesDto
{
    public Guid CardId { get; set; }
    public string CardholderName { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = string.Empty;
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AverageAmount { get; set; }
    public decimal? ThisMonthAmount { get; set; }
    public decimal? LastMonthAmount { get; set; }
    public List<TransactionDto> RecentTransactions { get; set; } = new();
    public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
}
