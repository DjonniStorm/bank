using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.BusinessLogicContracts;

public interface IClientBusinessLogicContract
{
    List<ClientDataModel> GetAllClients();

    List<ClientDataModel> GetClientByClerk(string clerkId);

    ClientDataModel GetClientByData(string data);

    void InsertClient(ClientDataModel clientDataModel);

    void UpdateClient(ClientDataModel clientDataModel);
}
