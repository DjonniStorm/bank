using BankContracts.Infrastructure;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace BankDatabase;

internal class BankDbContext(IConfigurationDatabase configurationDatabase) : DbContext
{
    private readonly IConfigurationDatabase? _configurationDatabase = configurationDatabase;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configurationDatabase?.ConnectionString, o => o.SetPostgresVersion(14, 2));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Clerk>()
            .HasIndex(x => x.Login)
            .IsUnique();

        modelBuilder.Entity<Clerk>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Clerk>()
            .HasIndex(x => x.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<CreditProgram>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Currency>()
            .HasIndex(x => x.Abbreviation)
            .IsUnique();

        modelBuilder.Entity<Storekeeper>()
            .HasIndex(x => x.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<Storekeeper>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Storekeeper>()
            .HasIndex(x => x.Login)
            .IsUnique();

        modelBuilder.Entity<Clerk>()
            .HasMany(x => x.Deposits)
            .WithOne(x => x.Clerk)
            .HasForeignKey(x => x.ClerkId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Clerk>()
            .HasMany(x => x.Clients)
            .WithOne(x => x.Clerk)
            .HasForeignKey(x => x.ClerkId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Clerk>()
            .HasMany(x => x.Replenishments)
            .WithOne(x => x.Clerk)
            .HasForeignKey(x => x.ClerkId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DepositCurrency>().HasKey(x => new { x.DepositId, x.CurrencyId });

        modelBuilder.Entity<ClientCreditProgram>().HasKey(x => new { x.ClientId, x.CreditProgramId });

        modelBuilder.Entity<DepositClient>().HasKey(x => new { x.DepositId, x.ClientId });

        modelBuilder.Entity<CreditProgramCurrency>().HasKey(x => new { x.CreditProgramId, x.CurrencyId });
    }

    public DbSet<Clerk> Clerks { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<CreditProgram> CreditPrograms { get; set; }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<Deposit> Deposits { get; set; }

    public DbSet<Period> Periods { get; set; }

    public DbSet<Replenishment> Replenishments { get; set; }

    public DbSet<Storekeeper> Storekeepers { get; set; }

    public DbSet<DepositCurrency> DepositCurrencies { get; set; }

    public DbSet<ClientCreditProgram> CreditProgramClients { get; set; }

    public DbSet<DepositClient> DepositClients { get; set; }

    public DbSet<CreditProgramCurrency> CurrencyCreditPrograms { get; set; }
}
