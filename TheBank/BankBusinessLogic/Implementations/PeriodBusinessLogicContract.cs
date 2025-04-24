using System.Text.Json;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

/// <summary>
/// реализация бизнес логики для сроков
/// </summary>
/// <param name="periodStorageContract">контракт сроков</param>
/// <param name="logger">логгер</param>
internal class PeriodBusinessLogicContract(
    IPeriodStorageContract periodStorageContract,
    ILogger logger
) : IPeriodBusinessLogicContract
{
    private readonly IPeriodStorageContract _periodStorageContract = periodStorageContract;

    private readonly ILogger _logger = logger;

    public List<PeriodDataModel> GetAllPeriods()
    {
        _logger.LogInformation("get all periods");
        return _periodStorageContract.GetList();
    }

    public List<PeriodDataModel> GetAllPeriodsByEndTime(DateTime fromDate, DateTime toDate)
    {
        if (toDate.IsDateNotOlder(toDate))
        {
            throw new IncorrectDatesException(fromDate, toDate);
        }
        return _periodStorageContract.GetList(fromDate, toDate).OrderBy(x => x.EndTime).ToList()
            ?? throw new NullListException(nameof(PeriodDataModel));
    }

    public List<PeriodDataModel> GetAllPeriodsByStartTime(DateTime fromDate, DateTime toDate)
    {
        if (toDate.IsDateNotOlder(toDate))
        {
            throw new IncorrectDatesException(fromDate, toDate);
        }
        return _periodStorageContract.GetList(fromDate, toDate).OrderBy(x => x.StartTime).ToList()
            ?? throw new NullListException(nameof(PeriodDataModel));
    }

    public List<PeriodDataModel> GetAllPeriodsByStorekeeper(string storekeeperId)
    {
        _logger.LogInformation("GetAllPeriodsByStorekeeper params: {storekeeperId}", storekeeperId);
        if (storekeeperId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(storekeeperId));
        }
        if (!storekeeperId.IsGuid())
        {
            throw new ValidationException(
                "The value in the field storekeeperId is not a unique identifier."
            );
        }
        return _periodStorageContract.GetList(storekeeperId: storekeeperId)
            ?? throw new NullListException($"{storekeeperId}");
    }

    public PeriodDataModel GetPeriodByData(string data)
    {
        _logger.LogInformation($"GetPeriodByData by data: {data}");
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _periodStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        throw new ElementNotFoundException($"element not found: {data}");
    }

    public void InsertPeriod(PeriodDataModel periodataModel)
    {
        _logger.LogInformation("Insert period: {period}", JsonSerializer.Serialize(periodataModel));
        ArgumentNullException.ThrowIfNull(periodataModel);
        periodataModel.Validate();
        _periodStorageContract.AddElement(periodataModel);
    }

    public void UpdatePeriod(PeriodDataModel periodataModel)
    {
        _logger.LogInformation("Update period: {period}", JsonSerializer.Serialize(periodataModel));
        ArgumentNullException.ThrowIfNull(periodataModel);
        periodataModel.Validate();
        _periodStorageContract.UpdElement(periodataModel);
    }
}
