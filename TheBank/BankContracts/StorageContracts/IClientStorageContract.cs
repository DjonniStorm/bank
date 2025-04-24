using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface IClientStorageContract
{
    List<ClientDataModel> GetList(string? clerkId = null);

    ClientDataModel? GetElementById(string id);

    ClientDataModel? GetElementByName(string name);

    ClientDataModel? GetElementBySurname(string surname);

    void AddElement(ClientDataModel clientDataModel);

    void UpdElement(ClientDataModel clientDataModel);
}
