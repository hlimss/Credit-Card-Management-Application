using System.ComponentModel.DataAnnotations;

namespace CreditCardManagement.API.Models;

public class User
{
    public Guid Id { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(50)]
    public string? Provider { get; set; } // "Facebook", "Google", or null for local
    
    [StringLength(200)]
    public string? ProviderId { get; set; } // External provider user ID
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
}

