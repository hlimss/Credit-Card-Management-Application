using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManagement.API.Models;

public class Loan
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string LoanType { get; set; } = string.Empty; // Personal, Mortgage, Auto, Business, etc.
    
    [Required]
    [StringLength(200)]
    public string LoanName { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrincipalAmount { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal RemainingAmount { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal InterestRate { get; set; } // Annual interest rate percentage
    
    [Required]
    public int TermMonths { get; set; }
    
    [Required]
    public int RemainingMonths { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyPayment { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime NextPaymentDate { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Paid, Defaulted, Cancelled
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
