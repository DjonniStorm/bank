using Microsoft.EntityFrameworkCore;
using BankDatabase.Models;
using BankDatabase;
using System.Xml.Linq;

namespace BankTests.Infrastructure;

internal static class BankDbContextExtension
{
    public static Clerk InsertClerkToDatabaseAndReturn(this BankDbContext dbContext, string? id = null, string? name = "vasya", string? surname = "petrov", string? middlename = "petrovich", string? login = "vasyapupkin", string? passwd = "*******", string? email = "email@email.com", string? phone = "+7-777-777-77-77")
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
            PhoneNumber = phone
        };
        dbContext.Clerks.Add( clerck );
        dbContext.SaveChanges();
        return clerck;
    }

    public static Client InsertClientToDatabaseAndReturn(this BankDbContext dbContext, string? id = null, string? name = "slava", string? surname = "fomichev", decimal balance = 1_000_000, string? clerkId = null)
    {
        var client = new Client()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            Name = name,
            Surname = surname,
            Balance = balance,
            ClerkId = clerkId ?? Guid.NewGuid().ToString(),
        };
        dbContext.Clients.Add( client );
        dbContext.SaveChanges();
        return client;
    }

    public static CreditProgram InsertCreditProgramToDatabaseAndReturn(this BankDbContext dbContext, string? id = null, string? name = "bankrot", decimal cost = 1_000_000, decimal maxCost = 10_000_000, string? storeleeperId = null, string? periodId = null)
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
        return creditProgram;
    }

    public static Storekeeper InsertStorekeeperToDatabaseAndReturn(this BankDbContext dbContext, 
        string? id = null, string? name = "slava", string? surname = "fomichev", string?  middlename = "sergeevich", string? login = "xomyak", string? password = "****", string? email = "email@email.com", string? phone = "+9-888-888-88-88")
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

    public static Period InsertPeriodToDatabaseAndReturn(this BankDbContext dbContext, string? id = null, DateTime? start = null, DateTime? end = null, string? storekeeperId = null)
    {
        var period = new Period()
        {
            Id = id ?? Guid.NewGuid().ToString(),
            StartTime = start ?? DateTime.UtcNow,
            EndTime = end ?? DateTime.UtcNow,
            StorekeeperId = storekeeperId ?? Guid.NewGuid().ToString()
        };
        dbContext.Periods.Add(period);
        dbContext.SaveChanges();
        return period;
    }

    public static Currency InsertCurrencyToDatabaseAndReturn(this BankDbContext dbContext, string? id = null, string? name = "pop", string? abbreviation = "rub", decimal cost = 10, string? storekeeperId = null)
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

    public static Deposit InsertDepositToDatabaseAndReturn(this BankDbContext dbContext, string? id = null, float interestRate = 1f, decimal cost = 10, int period = 1, string? clerkId = null)
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

    public static void RemoveCurrenciesFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"Currencies\" CASCADE");

    public static void RemoveClientsFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"Clients\" CASCADE");

    public static void RemoveStorekeepersFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"Storekeepers\" CASCADE");

    public static void RemovePeriodsFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"Periods\" CASCADE");

    public static void RemoveClerksFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"Clerks\" CASCADE");

    public static void RemoveCreditProgramsFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"CreditPrograms\" CASCADE");
    
    public static void RemoveDepositsFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"Deposits\" CASCADE");

    public static Client? GetClientFromDatabase(this BankDbContext dbContext, string id) => dbContext.Clients.FirstOrDefault(x => x.Id == id);

    public static Clerk? GetClerkFromDatabase(this BankDbContext dbContext, string id) => dbContext.Clerks.FirstOrDefault(x => x.Id == id);

    public static CreditProgram? GetCreditProgramFromDatabase(this BankDbContext dbContext, string id) => dbContext.CreditPrograms.FirstOrDefault(x => x.Id == id);

    public static Currency? GetCurrencyFromDatabase(this BankDbContext dbContext, string id) => dbContext.Currencies.FirstOrDefault(x => x.Id == id);

    public static Deposit? GetDepositFromDatabase(this BankDbContext dbContext, string id) => dbContext.Deposits.FirstOrDefault(x => x.Id == id);
    
    private static void ExecuteSqlRaw(this BankDbContext dbContext, string command) => dbContext.Database.ExecuteSqlRaw(command);
}
