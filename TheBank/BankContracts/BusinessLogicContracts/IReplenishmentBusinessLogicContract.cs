using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IReplenishmentBusinessLogicContract
{
    List<ReplenishmentDataModel> GetAllReplenishments();

    ReplenishmentDataModel GetReplenishmentByData(string data);

    List<ReplenishmentDataModel> GetAllReplenishmentsByDate(DateTime fromDate, DateTime toDate);

    List<ReplenishmentDataModel> GetAllReplenishmentsByDeposit(string depositId);

    List<ReplenishmentDataModel> GetAllReplenishmentsByClerk(string clerkId);

    void InsertReplenishment(ReplenishmentDataModel replenishmentataModel);

    void UpdateReplenishment(ReplenishmentDataModel replenishmentataModel);
}
