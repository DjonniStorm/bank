using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IClerkBusinessLogicContract
{
    List<ClerkDataModel> GetAllClerks();

    ClerkDataModel GetClerkByData(string data);

    void InsertClerk(ClerkDataModel clerkDataModel);

    void UpdateClerk(ClerkDataModel clerkDataModel);
}
