using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createDto, Guid userId)
    {
        // Si un CreditCardId est fourni, vérifier qu'il appartient à l'utilisateur
        if (createDto.CreditCardId.HasValue)
        {
            var card = await _context.CreditCards
                .FirstOrDefaultAsync(c => c.Id == createDto.CreditCardId.Value && c.UserId == userId);

            if (card == null)
            {
                throw new UnauthorizedAccessException("Carte non trouvée ou accès refusé");
            }

            // Vérifier que la balance est suffisante
            if (card.Balance.HasValue && card.Balance.Value < createDto.Amount)
            {
                throw new InvalidOperationException($"Solde insuffisant. Solde actuel: {card.Balance.Value:C}, Montant requis: {createDto.Amount:C}");
            }

            // Déduire le montant de la balance de la carte
            if (card.Balance.HasValue)
            {
                card.Balance -= createDto.Amount;
                card.UpdatedAt = DateTime.UtcNow;
                // Marquer la carte comme modifiée dans le contexte
                _context.CreditCards.Update(card);
            }
        }

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreditCardId = createDto.CreditCardId,
            PaymentType = createDto.PaymentType,
            MerchantName = createDto.MerchantName,
            ReferenceNumber = createDto.ReferenceNumber,
            Amount = createDto.Amount,
            Currency = createDto.Currency,
            Description = createDto.Description,
            PaymentDate = createDto.PaymentDate ?? DateTime.UtcNow,
            Status = "Completed", // Pour simplifier, on marque comme complété immédiatement
            ReceiptNumber = $"REC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return MapToDto(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetUserPaymentsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Payments
            .Where(p => p.UserId == userId);

        if (startDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate <= endDate.Value);
        }

        var payments = await query
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return payments.Select(MapToDto);
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId, Guid userId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == userId);

        return payment == null ? null : MapToDto(payment);
    }

    public async Task<IEnumerable<string>> GetAvailablePaymentTypesAsync()
    {
        // Retourner les types de paiement disponibles
        return await Task.FromResult(new List<string>
        {
            "Vignette",
            "Abonnement Téléphonique",
            "Facture Électricité",
            "Facture Eau",
            "Facture Internet",
            "Facture TV",
            "Abonnement Streaming",
            "Assurance",
            "Impôts",
            "Autre"
        });
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            CreditCardId = payment.CreditCardId,
            PaymentType = payment.PaymentType,
            MerchantName = payment.MerchantName,
            ReferenceNumber = payment.ReferenceNumber,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Description = payment.Description,
            PaymentDate = payment.PaymentDate,
            Status = payment.Status,
            ReceiptNumber = payment.ReceiptNumber,
            CreatedAt = payment.CreatedAt
        };
    }
}
