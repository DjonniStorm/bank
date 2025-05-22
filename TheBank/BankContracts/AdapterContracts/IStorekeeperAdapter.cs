using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;

namespace BankContracts.AdapterContracts;

/// <summary>
/// контракт адаптера для кладовщика
/// </summary>
public interface IStorekeeperAdapter
{
    StorekeeperOperationResponse GetList();

    StorekeeperOperationResponse GetElement(string data);

    StorekeeperOperationResponse RegisterStorekeeper(StorekeeperBindingModel storekeeperModel);

    StorekeeperOperationResponse ChangeStorekeeperInfo(StorekeeperBindingModel storekeeperModel);

    StorekeeperOperationResponse Login(LoginBindingModel storekeeperModel, out string token);
}
