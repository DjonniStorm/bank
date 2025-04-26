using Microsoft.EntityFrameworkCore;
using BankDatabase.Models;
using BankDatabase;

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

    public static void RemoveClerksFromDatabase(this BankDbContext dbContext) => dbContext.ExecuteSqlRaw("TRUNCATE \"Clerks\" CASCADE");

    public static Clerk? GetClerkFromDatabase(this BankDbContext dbContext, string id) => dbContext.Clerks.FirstOrDefault(x => x.Id == id);

    private static void ExecuteSqlRaw(this BankDbContext dbContext, string command) => dbContext.Database.ExecuteSqlRaw(command);
}
