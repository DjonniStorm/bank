using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;

namespace BankContracts.AdapterContracts;

public interface ICreditProgramAdapter
{
    CreditProgramOperationResponse GetList();

    CreditProgramOperationResponse GetElement(string data);

    CreditProgramOperationResponse GetListByStorekeeper(string storekeeperId);

    CreditProgramOperationResponse GetListByPeriod(string periodId);

    CreditProgramOperationResponse RegisterCreditProgram(CreditProgramBindingModel creditProgramModel);

    CreditProgramOperationResponse ChangeCreditProgramInfo(CreditProgramBindingModel creditProgramModel);
}
