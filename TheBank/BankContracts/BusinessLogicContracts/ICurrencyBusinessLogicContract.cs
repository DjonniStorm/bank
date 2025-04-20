using BankContracts.DataModels;
namespace BankContracts.BusinessLogicContracts;

public interface ICurrencyBusinessLogicContract
{
    List<CurrencyDataModel> GetAllCurrencys();

    List<CurrencyDataModel> GetCurrencyByStorekeeper(string storekeeperId);

    CurrencyDataModel GetCurrencyByData(string data);

    void InsertCurrency(CurrencyDataModel currencyDataModel);

    void UpdateCurrency(CurrencyDataModel currencyDataModel);
}
