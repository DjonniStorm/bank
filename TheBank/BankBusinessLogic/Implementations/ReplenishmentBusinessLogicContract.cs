using System.Text.Json;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

/// <summary>
/// реализация бизнес логики для пополнения
/// </summary>
/// <param name="replenishmentStorageContract">контракт пополнения</param>
/// <param name="logger">логгер</param>
internal class ReplenishmentBusinessLogicContract(
    IReplenishmentStorageContract replenishmentStorageContract,
    ILogger logger
) : IReplenishmentBusinessLogicContract
{
    private readonly IReplenishmentStorageContract _replenishmentStorageContract =
        replenishmentStorageContract;

    private readonly ILogger _logger = logger;

    public List<ReplenishmentDataModel> GetAllReplenishments()
    {
        _logger.LogInformation("get all replenishments");
        return _replenishmentStorageContract.GetList();
    }

    public List<ReplenishmentDataModel> GetAllReplenishmentsByClerk(string clerkId)
    {
        _logger.LogInformation("GetAllReplenishmentsByClerk params: {clerkId}", clerkId);
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
        return _replenishmentStorageContract.GetList(clerkId: clerkId)
            ?? throw new NullListException($"{clerkId}");
    }

    public List<ReplenishmentDataModel> GetAllReplenishmentsByDate(
        DateTime fromDate,
        DateTime toDate
    )
    {
        if (toDate.IsDateNotOlder(fromDate))
        {
            throw new IncorrectDatesException(fromDate, toDate);
        }
        return _replenishmentStorageContract.GetList(fromDate, toDate).ToList()
            ?? throw new NullListException(nameof(PeriodDataModel));
    }

    public List<ReplenishmentDataModel> GetAllReplenishmentsByDeposit(string depositId)
    {
        _logger.LogInformation("GetAllReplenishmentsByClerk params: {depositId}", depositId);
        if (depositId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(depositId));
        }
        if (!depositId.IsGuid())
        {
            throw new ValidationException(
                "The value in the field depositId is not a unique identifier."
            );
        }
        return _replenishmentStorageContract.GetList(depositId: depositId)
            ?? throw new NullListException($"{depositId}");
    }

    public ReplenishmentDataModel GetReplenishmentByData(string data)
    {
        _logger.LogInformation($"GetReplenishmentByData by data: {data}");
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _replenishmentStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        throw new ElementNotFoundException($"element not found: {data}");
    }

    public void InsertReplenishment(ReplenishmentDataModel replenishmentataModel)
    {
        _logger.LogInformation(
            "Insert replenishment: {replenishment}",
            JsonSerializer.Serialize(replenishmentataModel)
        );
        ArgumentNullException.ThrowIfNull(replenishmentataModel);
        replenishmentataModel.Validate();
        _replenishmentStorageContract.AddElement(replenishmentataModel);
    }

    public void UpdateReplenishment(ReplenishmentDataModel replenishmentataModel)
    {
        _logger.LogInformation(
            "Update replenishment: {replenishment}",
            JsonSerializer.Serialize(replenishmentataModel)
        );
        ArgumentNullException.ThrowIfNull(replenishmentataModel);
        replenishmentataModel.Validate();
        _replenishmentStorageContract.UpdElement(replenishmentataModel);
    }
}
