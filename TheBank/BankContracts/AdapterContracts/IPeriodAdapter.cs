using BankContracts.AdapterContracts.OperationResponses;
using BankContracts.BindingModels;

namespace BankContracts.AdapterContracts;

public interface IPeriodAdapter
{
    PeriodOperationResponse GetList();

    PeriodOperationResponse GetElement(string data);

    PeriodOperationResponse GetListByStorekeeper(string storekeeperId);

    PeriodOperationResponse GetListByStartTime(DateTime fromDate);

    PeriodOperationResponse GetListByEndTime(DateTime toDate);

    PeriodOperationResponse RegisterPeriod(PeriodBindingModel periodModel);

    PeriodOperationResponse ChangePeriodInfo(PeriodBindingModel periodModel);
}
