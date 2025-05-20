using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;

namespace BankContracts.AdapterContracts;

public interface IReplenishmentAdapter
{
    ReplenishmentOperationResponse GetList();

    ReplenishmentOperationResponse GetElement(string data);

    ReplenishmentOperationResponse GetListByClerk(string clerkId);

    ReplenishmentOperationResponse GetListByDeposit(string depositId);

    ReplenishmentOperationResponse GetListByDate(DateTime fromDate, DateTime toDate);

    ReplenishmentOperationResponse RegisterReplenishment(ReplenishmentBindingModel replenishmentModel);

    ReplenishmentOperationResponse ChangeReplenishmentInfo(ReplenishmentBindingModel replenishmentModel);
}
