namespace BankContracts.Infrastructure;

/// <summary>
/// интерфейс для подключения к бд
/// хз зачем он нужен тут в контрактах а не в самой бд
/// </summary>
public interface IConfigurationDatabase
{
    public string ConnectionString { get; }
}
