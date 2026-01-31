using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditCardManagement.API.Models;

public class CreditCard
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(500)] // Increased to accommodate Base64 encrypted card numbers
    public string CardNumber { get; set; } = string.Empty; // Encrypted
    
    [Required]
    [StringLength(100)]
    public string CardholderName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(5)]
    public string ExpirationDate { get; set; } = string.Empty; // MM/YY format
    
    [Required]
    [StringLength(100)] // Increased to accommodate Base64 encrypted CVV
    public string CVV { get; set; } = string.Empty; // Encrypted
    
    [StringLength(20)]
    public string CardType { get; set; } = string.Empty; // Visa, MasterCard, etc.
    
    [StringLength(50)]
    public string Category { get; set; } = "Personnel"; // Travail, Personnel, Abonnements, Voyage
    
    [StringLength(500)]
    public string? Tags { get; set; } // Comma-separated tags
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Balance { get; set; } // Card balance or limit amount
    
    public bool IsActive { get; set; } = true; // For temporary blocking
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

