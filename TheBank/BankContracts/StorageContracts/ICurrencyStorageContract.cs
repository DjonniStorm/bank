using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface ICurrencyStorageContract
{
    List<CurrencyDataModel> GetList(string? storekeeperId = null);

    Task<List<CurrencyDataModel>> GetListAsync(DateTime startDate, DateTime endDate, CancellationToken ct);

    CurrencyDataModel? GetElementById(string id);

    CurrencyDataModel? GetElementByAbbreviation(string abbreviation);

    void AddElement(CurrencyDataModel currencyDataModel);

    void UpdElement(CurrencyDataModel currencyDataModel);
}
