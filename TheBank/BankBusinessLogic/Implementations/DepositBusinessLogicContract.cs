using System.Text.Json;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

/// <summary>
/// реализация бизнес логики для вклада
/// </summary>
/// <param name="depositStorageContract">контракт вклада</param>
/// <param name="logger">логгер</param>
internal class DepositBusinessLogicContract(
    IDepositStorageContract depositStorageContract,
    ILogger logger
) : IDepositBusinessLogicContract
{
    private readonly IDepositStorageContract _depositStorageContract = depositStorageContract;

    private readonly ILogger _logger = logger;

    public List<DepositDataModel> GetAllDeposits()
    {
        _logger.LogInformation("get all deposits");
        return _depositStorageContract.GetList();
    }

    public List<DepositDataModel> GetDepositByClerk(string clerkId)
    {
        _logger.LogInformation("GetDepositByClerk params: {clerkId}", clerkId);
        if (clerkId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(clerkId));
        }
        if (!clerkId.IsGuid())
        {
            throw new ValidationException(
                "The value in the field clerkId is not a unique identifier."
            );
        }
        return _depositStorageContract.GetList(clerkId: clerkId)
            ?? throw new NullListException($"{clerkId}");
    }

    public DepositDataModel GetDepositByData(string data)
    {
        _logger.LogInformation($"GetDepositByData by data: {data}");
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _depositStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        return _depositStorageContract.GetElementByName(data)
            ?? throw new ElementNotFoundException($"element not found: {data}");
    }

    public void InsertDeposit(DepositDataModel depositDataModel)
    {
        _logger.LogInformation(
            "Insert credit program: {credit program}",
            JsonSerializer.Serialize(depositDataModel)
        );
        ArgumentNullException.ThrowIfNull(depositDataModel);
        depositDataModel.Validate();
        _depositStorageContract.AddElement(depositDataModel);
    }

    public void UpdateDeposit(DepositDataModel depositDataModel)
    {
        _logger.LogInformation("Update credit program: {credit program}", depositDataModel);
        ArgumentNullException.ThrowIfNull(depositDataModel);
        depositDataModel.Validate();
        _depositStorageContract.UpdElement(depositDataModel);
    }
}
