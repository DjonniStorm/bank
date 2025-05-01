using BankContracts.Infrastructure;

namespace BankWebApi.Infrastructure;

public class ConfigurationDatabase(IConfiguration configuration) : IConfigurationDatabase
{
    private readonly Lazy<DataBaseSettings> _dataBaseSettings = new(() =>
    {
        return configuration.GetSection("DataBaseSettings").Get<DataBaseSettings>()
            ?? throw new InvalidDataException("DataBaseSettings section is missing or invalid.");
    });

    public string ConnectionString => _dataBaseSettings.Value.ConnectionString;
}
