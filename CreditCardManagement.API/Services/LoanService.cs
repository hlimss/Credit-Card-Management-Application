using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Data;
using CreditCardManagement.API.DTOs;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Services;

public class LoanService : ILoanService
{
    private readonly ApplicationDbContext _context;

    public LoanService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createDto, Guid userId)
    {
        var startDate = createDto.StartDate ?? DateTime.UtcNow;
        
        // Calculer le paiement mensuel (formule simplifiée)
        var monthlyRate = (double)(createDto.InterestRate / 100 / 12);
        var principalAmount = (double)createDto.PrincipalAmount;
        var termMonths = createDto.TermMonths;
        var monthlyPayment = principalAmount * 
            (monthlyRate * Math.Pow(1 + monthlyRate, termMonths)) /
            (Math.Pow(1 + monthlyRate, termMonths) - 1);

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LoanType = createDto.LoanType,
            LoanName = createDto.LoanName,
            PrincipalAmount = createDto.PrincipalAmount,
            RemainingAmount = createDto.PrincipalAmount,
            InterestRate = createDto.InterestRate,
            TermMonths = createDto.TermMonths,
            RemainingMonths = createDto.TermMonths,
            MonthlyPayment = (decimal)monthlyPayment,
            StartDate = startDate,
            NextPaymentDate = startDate.AddMonths(1),
            Status = "Active",
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(Guid userId)
    {
        var loans = await _context.Loans
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return loans.Select(MapToDto);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid loanId, Guid userId)
    {
        var loan = await _context.Loans
            .FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId);

        return loan == null ? null : MapToDto(loan);
    }

    public async Task<LoanDto> MakePaymentAsync(Guid loanId, decimal amount, Guid userId)
    {
        var loan = await _context.Loans
            .FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId);

        if (loan == null)
        {
            throw new UnauthorizedAccessException("Prêt non trouvé ou accès refusé");
        }

        if (loan.Status != "Active")
        {
            throw new InvalidOperationException("Ce prêt n'est plus actif");
        }

        // Calculer l'intérêt mensuel
        var monthlyInterest = loan.RemainingAmount * (loan.InterestRate / 100 / 12);
        var principalPayment = amount - monthlyInterest;

        // Mettre à jour le prêt
        loan.RemainingAmount -= principalPayment;
        loan.RemainingMonths -= 1;
        loan.NextPaymentDate = loan.NextPaymentDate.AddMonths(1);
        loan.UpdatedAt = DateTime.UtcNow;

        if (loan.RemainingAmount <= 0)
        {
            loan.RemainingAmount = 0;
            loan.Status = "Paid";
            loan.RemainingMonths = 0;
        }

        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();

        return MapToDto(loan);
    }

    public async Task<bool> DeleteLoanAsync(Guid loanId, Guid userId)
    {
        var loan = await _context.Loans
            .FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId);

        if (loan == null)
        {
            return false;
        }

        _context.Loans.Remove(loan);
        await _context.SaveChangesAsync();

        return true;
    }

    private static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            LoanType = loan.LoanType,
            LoanName = loan.LoanName,
            PrincipalAmount = loan.PrincipalAmount,
            RemainingAmount = loan.RemainingAmount,
            InterestRate = loan.InterestRate,
            TermMonths = loan.TermMonths,
            RemainingMonths = loan.RemainingMonths,
            MonthlyPayment = loan.MonthlyPayment,
            StartDate = loan.StartDate,
            NextPaymentDate = loan.NextPaymentDate,
            Status = loan.Status,
            Description = loan.Description,
            CreatedAt = loan.CreatedAt
        };
    }
}
