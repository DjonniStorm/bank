using System.Text.Json;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

/// <summary>
/// реализация бизнес логики для кредитной программы
/// </summary>
/// <param name="cpStorageContract">контракт хранилища кредитной программы</param>
/// <param name="logger">логгер</param>
internal class CreditProgramBusinessLogicContract(
    ICreditProgramStorageContract cpStorageContract,
    ILogger logger
) : ICreditProgramBusinessLogicContract
{
    private readonly ICreditProgramStorageContract _creditProgramStorageContract =
        cpStorageContract;

    private readonly ILogger _logger = logger;

    public List<CreditProgramDataModel> GetAllCreditPrograms()
    {
        _logger.LogInformation("get all credit programs");
        return _creditProgramStorageContract.GetList();
    }

    public CreditProgramDataModel GetCreditProgramByData(string data)
    {
        _logger.LogInformation($"Get creadit program by data: {data}");
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _creditProgramStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        throw new ElementNotFoundException($"element not found: {data}");
    }

    public List<CreditProgramDataModel> GetCreditProgramByPeriod(string periodId)
    {
        _logger.LogInformation("GetCreditProgramByPeriod params: {periodId}", periodId);
        if (periodId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(periodId));
        }
        if (!periodId.IsGuid())
        {
            throw new ValidationException(
                "The value in the field periodId is not a unique identifier."
            );
        }
        return _creditProgramStorageContract.GetList(periodId: periodId)
            ?? throw new NullListException($"{periodId}");
    }

    public List<CreditProgramDataModel> GetCreditProgramByStorekeeper(string storekeeperId)
    {
        _logger.LogInformation(
            "GetCreditProgramByStorekeeper params: {storekeeperId}",
            storekeeperId
        );
        if (storekeeperId.IsEmpty())
        {
            throw new ArgumentNullException(nameof(storekeeperId));
        }
        if (!storekeeperId.IsGuid())
        {
            throw new ValidationException(
                "The value in the field clerkId is not a unique identifier."
            );
        }
        return _creditProgramStorageContract.GetList(storekeeperId: storekeeperId)
            ?? throw new NullListException($"{storekeeperId}");
    }

    public void InsertCreditProgram(CreditProgramDataModel creditProgramDataModel)
    {
        _logger.LogInformation(
            "Insert credit program: {credit program}",
            JsonSerializer.Serialize(creditProgramDataModel)
        );
        ArgumentNullException.ThrowIfNull(creditProgramDataModel);
        creditProgramDataModel.Validate();
        _creditProgramStorageContract.AddElement(creditProgramDataModel);
    }

    public void UpdateCreditProgram(CreditProgramDataModel creditProgramDataModel)
    {
        _logger.LogInformation("Update credit program: {credit program}", creditProgramDataModel);
        ArgumentNullException.ThrowIfNull(creditProgramDataModel);
        creditProgramDataModel.Validate();
        _creditProgramStorageContract.UpdElement(creditProgramDataModel);
    }
}
