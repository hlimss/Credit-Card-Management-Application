using System.ComponentModel.DataAnnotations;

namespace CreditCardManagement.API.DTOs;

public class CreditCardDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Card number is required")]
    [CreditCard(ErrorMessage = "Invalid card number format")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cardholder name is required")]
    [StringLength(100, ErrorMessage = "Cardholder name cannot exceed 100 characters")]
    public string CardholderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Expiration date is required")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Expiration date must be in MM/YY format")]
    public string ExpirationDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "CVV is required")]
    [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
    public string CVV { get; set; } = string.Empty;

    public string CardType { get; set; } = string.Empty;
    public string Category { get; set; } = "Personnel";
    public string? Tags { get; set; }
    public decimal? Balance { get; set; }
    public bool IsActive { get; set; } = true;
    public int DaysUntilExpiration { get; set; }
    public bool IsExpiringSoon { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateCreditCardDto
{
    [Required(ErrorMessage = "Card number is required")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cardholder name is required")]
    [StringLength(100, ErrorMessage = "Cardholder name cannot exceed 100 characters")]
    public string CardholderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Expiration date is required")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Expiration date must be in MM/YY format")]
    public string ExpirationDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "CVV is required")]
    [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
    public string CVV { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string Category { get; set; } = "Personnel";
    
    [StringLength(500)]
    public string? Tags { get; set; }
    
    [Range(0, 999999999.99, ErrorMessage = "Balance must be between 0 and 999999999.99")]
    public decimal? Balance { get; set; }
}

public class UpdateCreditCardDto
{
    public string? CardNumber { get; set; }

    [StringLength(100, ErrorMessage = "Cardholder name cannot exceed 100 characters")]
    public string? CardholderName { get; set; }

    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Expiration date must be in MM/YY format")]
    public string? ExpirationDate { get; set; }

    [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
    public string? CVV { get; set; }
    
    [StringLength(50)]
    public string? Category { get; set; }
    
    [StringLength(500)]
    public string? Tags { get; set; }
    
    [Range(0, 999999999.99, ErrorMessage = "Balance must be between 0 and 999999999.99")]
    public decimal? Balance { get; set; }
    
    public bool? IsActive { get; set; }
}
