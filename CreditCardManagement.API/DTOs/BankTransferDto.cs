namespace CreditCardManagement.API.DTOs;

public class BankTransferDto
{
    public Guid Id { get; set; }
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string BeneficiaryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MAD";
    public string? Description { get; set; }
    public string TransferType { get; set; } = "Internal";
    public DateTime TransferDate { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}

public class CreateBankTransferDto
{
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public string BeneficiaryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "MAD";
    public string? Description { get; set; }
    public string TransferType { get; set; } = "Internal";
    public DateTime? TransferDate { get; set; }
}
