using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManagement.API.Models;

public class BankTransfer
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FromAccount { get; set; } = string.Empty; // Numéro de compte ou carte
    
    [Required]
    [StringLength(100)]
    public string ToAccount { get; set; } = string.Empty; // Bénéficiaire
    
    [Required]
    [StringLength(200)]
    public string BeneficiaryName { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "MAD";
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(50)]
    public string TransferType { get; set; } = "Internal"; // Internal, External, International
    
    [Required]
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Cancelled
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
