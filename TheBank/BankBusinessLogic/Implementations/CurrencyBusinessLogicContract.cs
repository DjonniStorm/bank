using System.Text.Json;
using BankContracts.BusinessLogicContracts;
using BankContracts.DataModels;
using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.StorageContracts;
using Microsoft.Extensions.Logging;

namespace BankBusinessLogic.Implementations;

/// <summary>
/// реализация бизнес логики для валюты
/// </summary>
/// <param name="currencyStorageContract">контракт валюты</param>
/// <param name="logger">логгер</param>
internal class CurrencyBusinessLogicContract(
    ICurrencyStorageContract currencyStorageContract,
    ILogger logger
) : ICurrencyBusinessLogicContract
{
    private readonly ICurrencyStorageContract _currencyStorageContract = currencyStorageContract;

    private readonly ILogger _logger = logger;

    public List<CurrencyDataModel> GetAllCurrencys()
    {
        _logger.LogInformation("get all currencys programs");
        return _currencyStorageContract.GetList();
    }

    public CurrencyDataModel GetCurrencyByData(string data)
    {
        _logger.LogInformation($"Get currencys program by data: {data}");
        if (data.IsEmpty())
        {
            throw new ArgumentNullException(nameof(data));
        }
        if (data.IsGuid())
        {
            return _currencyStorageContract.GetElementById(data)
                ?? throw new ElementNotFoundException($"element not found: {data}");
        }
        return _currencyStorageContract.GetElementByAbbreviation(data)
            ?? throw new ElementNotFoundException($"element not found: {data}");
    }

    public List<CurrencyDataModel> GetCurrencyByStorekeeper(string storekeeperId)
    {
        _logger.LogInformation("GetCurrencyByStorekeeper params: {storekeeperId}", storekeeperId);
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
        return _currencyStorageContract.GetList(storekeeperId: storekeeperId)
            ?? throw new NullListException($"{storekeeperId}");
    }

    public void InsertCurrency(CurrencyDataModel currencyDataModel)
    {
        _logger.LogInformation(
            "Insert currency: {currency}",
            JsonSerializer.Serialize(currencyDataModel)
        );
        ArgumentNullException.ThrowIfNull(currencyDataModel);
        currencyDataModel.Validate();
        _currencyStorageContract.AddElement(currencyDataModel);
    }

    public void UpdateCurrency(CurrencyDataModel currencyDataModel)
    {
        _logger.LogInformation("Update currency: {currency}", currencyDataModel);
        ArgumentNullException.ThrowIfNull(currencyDataModel);
        currencyDataModel.Validate();
        _currencyStorageContract.UpdElement(currencyDataModel);
    }
}
