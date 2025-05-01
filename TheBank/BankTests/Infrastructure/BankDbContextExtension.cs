using System.Xml.Linq;
using BankDatabase;
using BankDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTests.Infrastructure;

internal static class BankDbContextExtension
{
    public static Clerk InsertClerkToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        string? name = "vasya",
        string? surname = "petrov",
        string? middlename = "petrovich",
        string? login = "vasyapupkin",
        string? passwd = "*******",
        string? email = "email@email.com",
        string? phone = "+7-777-777-77-77"
    )
    {
        var clerck = new Clerk()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Surname = surname,
            MiddleName = middlename,
            Login = login,
            Password = passwd,
            Email = email,
            PhoneNumber = phone,
        };
        dbContext.Clerks.Add(clerck);
        dbContext.SaveChanges();
        return clerck;
    }

    public static Client InsertClientToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        string? name = "slava",
        string? surname = "fomichev",
        decimal balance = 1_000_000,
        string? clerkId = null,
        List<(string clientId, string creditProgramId)>? creditProgramClients = null, // Item1 = ClientId Item2 = CreditProgramId
        List<(string depositId, string clientId)>? depositClients = null // Item1 = DepositId Item2 = ClientId
    )
    {
        var client = new Client()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Surname = surname,
            Balance = balance,
            ClerkId = clerkId ?? Guid.NewGuid().ToString(),
            DepositClients = [],
            CreditProgramClients = [],
        };
        if (creditProgramClients is not null)
        {
            foreach (var (clientId, creditProgramId) in creditProgramClients)
            {
                dbContext.CreditProgramClients.Add(
                    new ClientCreditProgram
                    {
                        ClientId = clientId,
                        CreditProgramId = creditProgramId,
                    }
                );
            }
        }
        if (depositClients is not null)
        {
            foreach (var (depositId, clientId) in depositClients)
            {
                dbContext.DepositClients.Add(
                    new DepositClient { ClientId = clientId, DepositId = depositId }
                );
            }
        }
        dbContext.Clients.Add(client);
        dbContext.SaveChanges();
        return client;
    }

    public static CreditProgram InsertCreditProgramToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        string? name = "bankrot",
        decimal cost = 1_000_000,
        decimal maxCost = 10_000_000,
        string? storeleeperId = null,
        string? periodId = null,
        List<(string currencyId, string creditProgramId)>? creditProgramCurrency = null // Item1 = ClientId Item2 = CreditProgramId
    )
    {
        var creditProgram = new CreditProgram()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Cost = cost,
            MaxCost = maxCost,
            StorekeeperId = storeleeperId ?? Guid.NewGuid().ToString(),
            PeriodId = periodId ?? Guid.NewGuid().ToString(),
        };
        dbContext.CreditPrograms.Add(creditProgram);
        dbContext.SaveChanges();
        if (creditProgramCurrency is not null)
        {
            foreach (var (currencyId, creditProgramId) in creditProgramCurrency)
            {
                dbContext.CurrencyCreditPrograms.Add(
                    new CreditProgramCurrency
                    {
                        CurrencyId = currencyId,
                        CreditProgramId = creditProgram.Id,
                    }
                );
            }
        }
        dbContext.SaveChanges();
        return creditProgram;
    }

    public static Storekeeper InsertStorekeeperToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        string? name = "slava",
        string? surname = "fomichev",
        string? middlename = "sergeevich",
        string? login = "xomyak",
        string? password = "****",
        string? email = "email@email.com",
        string? phone = "+9-888-888-88-88"
    )
    {
        var storekeeper = new Storekeeper()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Surname = surname,
            MiddleName = middlename,
            Login = login,
            Password = password,
            Email = email,
            PhoneNumber = phone,
        };
        dbContext.Storekeepers.Add(storekeeper);
        dbContext.SaveChanges();
        return storekeeper;
    }

    public static Period InsertPeriodToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        DateTime? start = null,
        DateTime? end = null,
        string? storekeeperId = null
    )
    {
        var period = new Period()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            StartTime = start ?? DateTime.UtcNow,
            EndTime = end ?? DateTime.UtcNow,
            StorekeeperId = storekeeperId ?? Guid.NewGuid().ToString(),
        };
        dbContext.Periods.Add(period);
        dbContext.SaveChanges();
        return period;
    }

    public static Currency InsertCurrencyToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        string? name = "pop",
        string? abbreviation = "rub",
        decimal cost = 10,
        string? storekeeperId = null
    )
    {
        var currency = new Currency()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Abbreviation = abbreviation,
            Cost = cost,
            StorekeeperId = storekeeperId ?? Guid.NewGuid().ToString(),
        };
        dbContext.Currencies.Add(currency);
        dbContext.SaveChanges();
        return currency;
    }

    public static Deposit InsertDepositToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        float interestRate = 1f,
        decimal cost = 10,
        int period = 1,
        string? clerkId = null
    )
    {
        var deposit = new Deposit()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            InterestRate = interestRate,
            Cost = cost,
            Period = period,
            ClerkId = clerkId ?? Guid.NewGuid().ToString(),
        };
        dbContext.Deposits.Add(deposit);
        dbContext.SaveChanges();
        return deposit;
    }

    public static Replenishment InsertReplenishmentToDatabaseAndReturn(
        this BankDbContext dbContext,
        string? id = null,
        decimal amount = 1,
        DateTime? date = null,
        string? depositId = null,
        string? clerkId = null
    )
    {
        var replenishment = new Replenishment()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Amount = amount,
            Date = date ?? DateTime.UtcNow,
            DepositId = depositId ?? Guid.NewGuid().ToString(),
            ClerkId = clerkId ?? Guid.NewGuid().ToString(),
        };
        dbContext.Replenishments.Add(replenishment);
        dbContext.SaveChanges();
        return replenishment;
    }

    public static void RemoveCurrenciesFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"Currencies\" CASCADE");

    public static void RemoveClientsFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"Clients\" CASCADE");

    public static void RemoveStorekeepersFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"Storekeepers\" CASCADE");

    public static void RemovePeriodsFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"Periods\" CASCADE");

    public static void RemoveClerksFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"Clerks\" CASCADE");

    public static void RemoveCreditProgramsFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"CreditPrograms\" CASCADE");

    public static void RemoveDepositsFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"Deposits\" CASCADE");

    public static void RemoveReplenishmentsFromDatabase(this BankDbContext dbContext) =>
        dbContext.ExecuteSqlRaw("TRUNCATE \"Replenishments\" CASCADE");

    public static Client? GetClientFromDatabase(this BankDbContext dbContext, string id) =>
        dbContext
            .Clients.Include(x => x.DepositClients)
            .Include(x => x.CreditProgramClients)
            .FirstOrDefault(x => x.Id == id);

    public static Clerk? GetClerkFromDatabase(this BankDbContext dbContext, string id) =>
        dbContext.Clerks.FirstOrDefault(x => x.Id == id);

    public static CreditProgram? GetCreditProgramFromDatabase(
        this BankDbContext dbContext,
        string id
    ) => dbContext.CreditPrograms.FirstOrDefault(x => x.Id == id);

    public static Currency? GetCurrencyFromDatabase(this BankDbContext dbContext, string id) =>
        dbContext.Currencies.FirstOrDefault(x => x.Id == id);

    public static Deposit? GetDepositFromDatabase(this BankDbContext dbContext, string id) =>
        dbContext.Deposits.FirstOrDefault(x => x.Id == id);

    public static Period? GetPeriodFromDatabase(this BankDbContext dbContext, string id) =>
        dbContext.Periods.FirstOrDefault(x => x.Id == id);

    public static Replenishment? GetReplenishmentFromDatabase(
        this BankDbContext dbContext,
        string id
    ) => dbContext.Replenishments.FirstOrDefault(x => x.Id == id);

    public static Storekeeper? GetStorekeeperFromDatabase(
        this BankDbContext dbContext,
        string id
    ) => dbContext.Storekeepers.FirstOrDefault(x => x.Id == id);

    private static void ExecuteSqlRaw(this BankDbContext dbContext, string command) =>
        dbContext.Database.ExecuteSqlRaw(command);
}
