using BankContracts.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankContracts.BusinessLogicContracts;

public interface IDepositBusinessLogicContract
{
    List<DepositDataModel> GetAllDeposits();
    
    List<DepositDataModel> GetDepositByClerk(string clerkId);

    DepositDataModel GetDepositByData(string data);

    void InsertDeposit(DepositDataModel depositDataModel);

    void UpdateDeposit(DepositDataModel depositDataModel);
}
