using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManagement.API.Models;

public class Transaction
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid CreditCardId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string MerchantName { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "USD";
    
    [StringLength(50)]
    public string Category { get; set; } = "Other"; // Food, Transport, Shopping, Entertainment, etc.
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    
    [StringLength(50)]
    public string TransactionType { get; set; } = "Expense"; // Expense, Refund, Payment
    
    [StringLength(100)]
    public string? Location { get; set; } // City, Country, or coordinates
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    [ForeignKey("CreditCardId")]
    public CreditCard CreditCard { get; set; } = null!;
}
