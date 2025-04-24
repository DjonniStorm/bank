using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface IPeriodStorageContract
{
    List<PeriodDataModel> GetList(DateTime? startDate = null, DateTime? endDate = null, string? storekeeperId = null);

    PeriodDataModel? GetElementById(string id);

    void AddElement(PeriodDataModel periodDataModel);

    void UpdElement(PeriodDataModel periodDataModel);
}
