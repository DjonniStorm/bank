using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface IReplenishmentStorageContract
{
    List<ReplenishmentDataModel> GetList();

    ReplenishmentDataModel? GetElementById(string id);

    void AddElement(ReplenishmentDataModel replenishmentDataModel);

    void UpdElement(ReplenishmentDataModel replenishmentDataModel);
}
