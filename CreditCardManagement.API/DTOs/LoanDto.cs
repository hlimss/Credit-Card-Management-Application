namespace CreditCardManagement.API.DTOs;

public class LoanDto
{
    public Guid Id { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public string LoanName { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public int RemainingMonths { get; set; }
    public decimal MonthlyPayment { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public string Status { get; set; } = "Active";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLoanDto
{
    public string LoanType { get; set; } = string.Empty;
    public string LoanName { get; set; } = string.Empty;
    public decimal PrincipalAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public DateTime? StartDate { get; set; }
    public string? Description { get; set; }
}
