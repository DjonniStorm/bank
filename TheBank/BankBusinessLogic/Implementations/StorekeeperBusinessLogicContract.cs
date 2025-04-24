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
/// реализация бизнес логики для кладовщика
/// </summary>
/// <param name="storekeeperStorageContract">контракт кладовщика</param>
/// <param name="logger">логгер</param>
internal class StorekeeperBusinessLogicContract(
    IStorekeeperStorageContract storekeeperStorageContract,
    ILogger logger
) : IStorekeeperBusinessLogicContract
{
    private readonly IStorekeeperStorageContract _storekeeperStorageContract =
        storekeeperStorageContract;

    private readonly ILogger _logger = logger;

    public List<StorekeeperDataModel> GetAllStorekeepers()
    {
        _logger.LogInformation("get all storekeepers");
        return _storekeeperStorageContract.GetList();
    }

    public StorekeeperDataModel GetStorekeeperByData(string data)
    {
        _logger.LogInformation($"Get storekeeper by data: {data}");
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _storekeeperStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        if (Regex.IsMatch(data, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
        {
            return _storekeeperStorageContract.GetElementByPhoneNumber(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        return _storekeeperStorageContract.GetElementByLogin(data)
            ?? throw new ElementNotFoundException($"element not found: {data}");
    }

    public void InsertStorekeeper(StorekeeperDataModel storekeeperDataModel)
    {
        _logger.LogInformation(
            "Insert storekeeper: {storekeeper}",
            JsonSerializer.Serialize(storekeeperDataModel)
        );
        ArgumentNullException.ThrowIfNull(storekeeperDataModel);
        storekeeperDataModel.Validate();
        _storekeeperStorageContract.AddElement(storekeeperDataModel);
    }

    public void UpdateStorekeeper(StorekeeperDataModel storekeeperDataModel)
    {
        _logger.LogInformation(
            "Update storekeeper: {storekeeper}",
            JsonSerializer.Serialize(storekeeperDataModel)
        );
        ArgumentNullException.ThrowIfNull(storekeeperDataModel);
        storekeeperDataModel.Validate();
        _storekeeperStorageContract.UpdElement(storekeeperDataModel);
    }
}
