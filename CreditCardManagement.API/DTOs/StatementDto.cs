namespace CreditCardManagement.API.DTOs;

public class StatementDto
{
    public Guid Id { get; set; }
    public Guid CreditCardId { get; set; }
    public string CreditCardName { get; set; } = string.Empty;
    public string StatementType { get; set; } = "Monthly";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public int TransactionCount { get; set; }
    public string? Notes { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateStatementDto
{
    public Guid CreditCardId { get; set; }
    public string StatementType { get; set; } = "Monthly";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
