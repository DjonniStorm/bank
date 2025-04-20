using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface ICreditProgramBusinessLogicContract
{
    List<CreditProgramDataModel> GetAllCreditPrograms();

    List<CreditProgramDataModel> GetCreditProgramByStorekeeper(string storekeeperId);

    List<CreditProgramDataModel> GetCreditProgramByPeriod(string periodId);

    CreditProgramDataModel GetCreditProgramByData(string data);  

    void InsertCreditProgram(CreditProgramDataModel creditProgramDataModel);

    void UpdateCreditProgram(CreditProgramDataModel creditProgramDataModel);
}
