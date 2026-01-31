using Microsoft.EntityFrameworkCore;
using CreditCardManagement.API.Models;

namespace CreditCardManagement.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BankTransfer> BankTransfers { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Statement> Statements { get; set; }
    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.ProviderId).HasMaxLength(200);
            entity.HasIndex(e => new { e.Provider, e.ProviderId });
        });

        // CreditCard configuration
        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.CreditCards)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
            entity.Property(e => e.Category).HasDefaultValue("Personnel");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.CreditCard)
                  .WithMany(c => c.Transactions) // Spécifier explicitement la propriété de navigation
                  .HasForeignKey(e => e.CreditCardId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.CreditCardId);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.Category);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // BankTransfer configuration
        modelBuilder.Entity<BankTransfer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TransferDate);
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction); // Changed from Cascade to avoid multiple cascade paths
            entity.HasOne(e => e.CreditCard)
                  .WithMany()
                  .HasForeignKey(e => e.CreditCardId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreditCardId);
            entity.HasIndex(e => e.PaymentDate);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PaymentType);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // Statement configuration
        modelBuilder.Entity<Statement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(e => e.CreditCard)
                  .WithMany()
                  .HasForeignKey(e => e.CreditCardId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreditCardId);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
            entity.Property(e => e.OpeningBalance).HasPrecision(18, 2);
            entity.Property(e => e.ClosingBalance).HasPrecision(18, 2);
            entity.Property(e => e.TotalCredits).HasPrecision(18, 2);
            entity.Property(e => e.TotalDebits).HasPrecision(18, 2);
        });

        // Loan configuration
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.NextPaymentDate);
            entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2);
            entity.Property(e => e.RemainingAmount).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 2);
            entity.Property(e => e.MonthlyPayment).HasPrecision(18, 2);
        });
    }
}

