using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public class BankTransferService : IBankTransferService
{
    private readonly ApplicationDbContext _context;

    public BankTransferService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BankTransferDto> CreateTransferAsync(CreateBankTransferDto createDto, Guid userId)
    {
        // Vérifier que le compte source existe (carte ou compte)
        if (!string.IsNullOrEmpty(createDto.FromAccount))
        {
            // Si c'est une carte, vérifier qu'elle appartient à l'utilisateur
            if (Guid.TryParse(createDto.FromAccount, out var cardId))
            {
                var card = await _context.CreditCards
                    .FirstOrDefaultAsync(c => c.Id == cardId && c.UserId == userId);
                
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
        }

        var transfer = new BankTransfer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FromAccount = createDto.FromAccount,
            ToAccount = createDto.ToAccount,
            BeneficiaryName = createDto.BeneficiaryName,
            Amount = createDto.Amount,
            Currency = createDto.Currency,
            Description = createDto.Description,
            TransferType = createDto.TransferType,
            TransferDate = createDto.TransferDate ?? DateTime.UtcNow,
            Status = "Completed", // Pour simplifier, on marque comme complété immédiatement
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.BankTransfers.Add(transfer);
        await _context.SaveChangesAsync();

        return MapToDto(transfer);
    }

    public async Task<IEnumerable<BankTransferDto>> GetUserTransfersAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.BankTransfers
            .Where(t => t.UserId == userId);

        if (startDate.HasValue)
        {
            query = query.Where(t => t.TransferDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.TransferDate <= endDate.Value);
        }

        var transfers = await query
            .OrderByDescending(t => t.TransferDate)
            .ToListAsync();

        return transfers.Select(MapToDto);
    }

    public async Task<BankTransferDto?> GetTransferByIdAsync(Guid transferId, Guid userId)
    {
        var transfer = await _context.BankTransfers
            .FirstOrDefaultAsync(t => t.Id == transferId && t.UserId == userId);

        return transfer == null ? null : MapToDto(transfer);
    }

    public async Task<bool> CancelTransferAsync(Guid transferId, Guid userId)
    {
        var transfer = await _context.BankTransfers
            .FirstOrDefaultAsync(t => t.Id == transferId && t.UserId == userId);

        if (transfer == null || transfer.Status != "Pending")
        {
            return false;
        }

        // Si le virement était depuis une carte, restaurer la balance
        if (!string.IsNullOrEmpty(transfer.FromAccount) && Guid.TryParse(transfer.FromAccount, out var cardId))
        {
            var card = await _context.CreditCards
                .FirstOrDefaultAsync(c => c.Id == cardId && c.UserId == userId);
            
            if (card != null && card.Balance.HasValue)
            {
                card.Balance += transfer.Amount;
                card.UpdatedAt = DateTime.UtcNow;
            }
        }

        transfer.Status = "Cancelled";
        transfer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static BankTransferDto MapToDto(BankTransfer transfer)
    {
        return new BankTransferDto
        {
            Id = transfer.Id,
            FromAccount = transfer.FromAccount,
            ToAccount = transfer.ToAccount,
            BeneficiaryName = transfer.BeneficiaryName,
            Amount = transfer.Amount,
            Currency = transfer.Currency,
            Description = transfer.Description,
            TransferType = transfer.TransferType,
            TransferDate = transfer.TransferDate,
            Status = transfer.Status,
            CreatedAt = transfer.CreatedAt
        };
    }
}
