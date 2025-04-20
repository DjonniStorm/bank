using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface ICreditProgramStorageContract
{
    List<CreditProgramDataModel> GetList();

    CreditProgramDataModel? GetElementById(string id);

    void AddElement(CreditProgramDataModel creditProgramDataModel);

    void UpdElement(CreditProgramDataModel creditProgramDataModel);
}
