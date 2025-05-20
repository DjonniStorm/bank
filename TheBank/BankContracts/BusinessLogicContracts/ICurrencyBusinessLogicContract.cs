using BankContracts.DataModels;
namespace BankContracts.BusinessLogicContracts;

public interface ICurrencyBusinessLogicContract
{
    List<CurrencyDataModel> GetAllCurrencies();

    List<CurrencyDataModel> GetCurrencyByStorekeeper(string storekeeperId);

    CurrencyDataModel GetCurrencyByData(string data);

    void InsertCurrency(CurrencyDataModel currencyDataModel);

    void UpdateCurrency(CurrencyDataModel currencyDataModel);
}
