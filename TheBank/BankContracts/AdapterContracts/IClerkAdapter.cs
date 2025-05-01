using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;

namespace BankContracts.AdapterContracts;

/// <summary>
/// контракт адаптера для клерка
/// </summary>
public interface IClerkAdapter
{
    ClerkOperationResponse GetList();

    ClerkOperationResponse GetElement(string data);

    ClerkOperationResponse RegisterClerk(ClerkBindingModel clerkDataModel);

    ClerkOperationResponse ChangeClerkInfo(ClerkBindingModel clerkDataModel);
}
