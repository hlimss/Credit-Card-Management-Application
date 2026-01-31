using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManagement.API.Models;

public class Statement
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid CreditCardId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string StatementType { get; set; } = "Monthly"; // Monthly, Quarterly, Annual
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ClosingBalance { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalCredits { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalDebits { get; set; }
    
    [Required]
    public int TransactionCount { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
    
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [ForeignKey("CreditCardId")]
    public CreditCard CreditCard { get; set; } = null!;
}
