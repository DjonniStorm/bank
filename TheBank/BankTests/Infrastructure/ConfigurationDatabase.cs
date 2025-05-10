using BankContracts.Infrastructure;

namespace BankTests.Infrastructure;

internal class ConfigurationDatabase : IConfigurationDatabase
{
    public string ConnectionString =>
        "Host=127.0.0.1;Port=5432;Database=TitanicTest;Username=postgres;Password=postgres;Include Error Detail=true";
}
