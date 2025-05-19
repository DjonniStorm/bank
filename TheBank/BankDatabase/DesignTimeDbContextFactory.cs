using BankContracts.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;

namespace BankDatabase;

//public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
//{
//    //public BankDbContext CreateDbContext(string[] args)
//    //{
//    //    return new BankDbContext(new ConfigurationDatabase());
//    //}
//}

internal class ConfigurationDatabase : IConfigurationDatabase
{
    public string ConnectionString => "Host=127.0.0.1;Port=5432;Database=BankTest;Username=postgres;Password=admin123;";
}