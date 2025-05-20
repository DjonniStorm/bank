using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;

namespace BankContracts.AdapterContracts;

/// <summary>
/// контракт адаптера для вклада
/// </summary>
public interface IDepositAdapter
{
    DepositOperationResponse GetList();

    DepositOperationResponse GetElement(string data);

    DepositOperationResponse GetListByClerk(string clerkId);

    DepositOperationResponse MakeDeposit(DepositBindingModel depositModel);

    DepositOperationResponse ChangeDepositInfo(DepositBindingModel depositModel);
}
