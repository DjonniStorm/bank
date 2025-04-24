using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface ICreditProgramStorageContract
{
    List<CreditProgramDataModel> GetList(string? storekeeperId = null, string? periodId = null);

    CreditProgramDataModel? GetElementById(string id);

    void AddElement(CreditProgramDataModel creditProgramDataModel);

    void UpdElement(CreditProgramDataModel creditProgramDataModel);
}
