using BankContracts.DataModels;
namespace BankContracts.StorageContracts;

public interface IStorekeeperStorageContract
{
    List<StorekeeperDataModel> GetList();

    StorekeeperDataModel? GetElementById(string id);

    StorekeeperDataModel? GetElementByPhoneNumber(string phoneNumber);

    StorekeeperDataModel? GetElementByLogin(string login);

    void AddElement(StorekeeperDataModel storekeeperDataModel);

    void UpdElement(StorekeeperDataModel storekeeperDataModel);
}
