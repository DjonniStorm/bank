using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface IPeriodStorageContract
{
    List<PeriodDataModel> GetList(DateTime startDate, DateTime endDate);

    PeriodDataModel? GetElementById(string id);

    void AddElement(PeriodDataModel periodDataModel);

    void UpdElement(PeriodDataModel periodDataModel);
}
