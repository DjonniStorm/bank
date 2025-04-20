using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IDepositBusinessLogicContract
{
    List<DepositDataModel> GetAllDeposits();
    
    List<DepositDataModel> GetDepositByClerk(string clerkId);

    DepositDataModel GetDepositByData(string data);

    void InsertDeposit(DepositDataModel depositDataModel);

    void UpdateDeposit(DepositDataModel depositDataModel);
}
