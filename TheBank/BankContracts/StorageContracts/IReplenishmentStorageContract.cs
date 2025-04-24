using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface IReplenishmentStorageContract
{
    List<ReplenishmentDataModel> GetList(DateTime? fromDate = null, DateTime? toDate = null, string? clerkId = null, string? depositId = null);

    ReplenishmentDataModel? GetElementById(string id);

    void AddElement(ReplenishmentDataModel replenishmentDataModel);

    void UpdElement(ReplenishmentDataModel replenishmentDataModel);
}
