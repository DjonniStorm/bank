using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IClientBusinessLogicContract
{
    List<ClientDataModel> GetAllClients();

    List<ClientDataModel> GetClientByClerk(string clerkId);

    ClientDataModel GetClientByData(string data);

    void InsertClient(ClientDataModel clientDataModel);

    void UpdateClient(ClientDataModel clientDataModel);
}
