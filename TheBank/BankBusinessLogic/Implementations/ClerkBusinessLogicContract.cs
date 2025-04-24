using System.Text.Json;
using System.Text.RegularExpressions;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

/// <summary>
/// реализация бизнес логики для клерка
/// </summary>
/// <param name="clerkStorageContract">контракт хранилища</param>
/// <param name="logger">логгер</param>
internal class ClerkBusinessLogicContract(
    IClerkStorageContract clerkStorageContract,
    ILogger logger
) : IClerkBusinessLogicContract
{
    private readonly IClerkStorageContract _clerkStorageContract = clerkStorageContract;
    private readonly ILogger _logger = logger;

    public List<ClerkDataModel> GetAllClerks()
    {
        _logger.LogInformation("get all clerks");
        return _clerkStorageContract.GetList();
    }

    public ClerkDataModel GetClerkByData(string data)
    {
        _logger.LogInformation($"Get clerk by data: {data}");
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _clerkStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        if (Regex.IsMatch(data, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
        {
            return _clerkStorageContract.GetElementByPhoneNumber(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        return _clerkStorageContract.GetElementByLogin(data)
            ?? throw new ElementNotFoundException($"element not found: {data}");
    }

    public void InsertClerk(ClerkDataModel clerkDataModel)
    {
        _logger.LogInformation(
            "Insert storekeeper: {storekeeper}",
            JsonSerializer.Serialize(clerkDataModel)
        );
        ArgumentNullException.ThrowIfNull(clerkDataModel);
        clerkDataModel.Validate();
        _clerkStorageContract.AddElement(clerkDataModel);
    }

    public void UpdateClerk(ClerkDataModel clerkDataModel)
    {
        _logger.LogInformation("Update clerk: {clerk}", JsonSerializer.Serialize(clerkDataModel));
        ArgumentNullException.ThrowIfNull(clerkDataModel);
        clerkDataModel.Validate();
        _clerkStorageContract.UpdElement(clerkDataModel);
    }
}
