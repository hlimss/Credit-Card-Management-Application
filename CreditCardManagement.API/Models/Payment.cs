using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManagement.API.Models;

public class Payment
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid? CreditCardId { get; set; } // Optionnel si paiement depuis compte
    
    [Required]
    [StringLength(100)]
    public string PaymentType { get; set; } = string.Empty; // Vignette, Abonnement, Facture, etc.
    
    [Required]
    [StringLength(200)]
    public string MerchantName { get; set; } = string.Empty; // Opérateur téléphonique, service, etc.
    
    [Required]
    [StringLength(100)]
    public string ReferenceNumber { get; set; } = string.Empty; // Numéro de référence du paiement
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = "MAD";
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Cancelled
    
    [StringLength(100)]
    public string? ReceiptNumber { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    [ForeignKey("CreditCardId")]
    public CreditCard? CreditCard { get; set; }
}
