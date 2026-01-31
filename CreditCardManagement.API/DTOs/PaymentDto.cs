namespace CreditCardManagement.API.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid? CreditCardId { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string MerchantName { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MAD";
    public string? Description { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ReceiptNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentDto
{
    public Guid? CreditCardId { get; set; }
    public string PaymentType { get; set; } = string.Empty; // Vignette, Abonnement, Facture, etc.
    public string MerchantName { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MAD";
    public string? Description { get; set; }
    public DateTime? PaymentDate { get; set; }
}
