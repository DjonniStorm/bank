using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.BusinessLogicContracts;

public interface IClerkBusinessLogicContract
{
    List<ClerkDataModel> GetAllClerks();

    ClerkDataModel GetClerkByData(string data);

    void InsertClerk(ClerkDataModel clerkDataModel);

    void UpdateClerk(ClerkDataModel clerkDataModel);
}
