using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface IDepositStorageContract
{
    List<DepositDataModel> GetList(string? clerkId = null);

    DepositDataModel? GetElementById(string id);

    DepositDataModel? GetElementByName(string name);

    void AddElement(DepositDataModel depositDataModel);

    void UpdElement(DepositDataModel depositDataModel);
}
