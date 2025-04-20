using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface IClerkStorageContract
{
    List<ClerkDataModel> GetList();

    ClerkDataModel? GetElementById(string id);

    ClerkDataModel? GetElementByPhoneNumber(string phoneNumber);

    ClerkDataModel? GetElementByLogin(string login);

    void AddElement(ClerkDataModel clerkDataModel);

    void UpdElement(ClerkDataModel clerkDataModel);
}
