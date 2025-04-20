using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IStorekeeperBusinessLogicContract
{
    List<StorekeeperDataModel> GetAllStorekeepers();

    StorekeeperDataModel GetStorekeeperByData(string data);

    void InsertStorekeeper(StorekeeperDataModel storekeeperDataModel);

    void UpdateStorekeeper(StorekeeperDataModel storekeeperDataModel);
}
