using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;

namespace BankContracts.AdapterContracts;

/// <summary>
/// контракт адаптера для валюыты
/// </summary>
public interface ICurrencyAdapter
{
    CurrencyOperationResponse GetList();

    CurrencyOperationResponse GetElement(string data);

    CurrencyOperationResponse GetListByStorekeeper(string storekeeperId);

    CurrencyOperationResponse MakeCurrency(CurrencyBindingModel currencyModel);

    CurrencyOperationResponse ChangeCurrencyInfo(CurrencyBindingModel currencyModel);
}
