using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.AdapterContracts;

/// <summary>
/// контракт адаптера для клиента
/// </summary>
public interface IClientAdapter
{
    ClientOperationResponse GetList();

    ClientOperationResponse GetElement(string data);

    ClientOperationResponse GetListByClerk(string clerkId);

    ClientOperationResponse RegisterClient(ClientBindingModel clientModel);

    ClientOperationResponse ChangeClientInfo(ClientBindingModel clientModel);
}
