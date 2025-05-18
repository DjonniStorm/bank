using BankContracts.BindingModels;
using BankContracts.BusinessLogicContracts;
using BankContracts.StorageContracts;
using BankContracts.ViewModels;

namespace BankBusinessLogic.Implementations;

public class ReportContract : IReportContract
{
    private readonly IClientStorageContract _clientStorage;
    private readonly IDepositStorageContract _depositStorage;
    private readonly ICreditProgramStorageContract _creditProgramStorage;
    private readonly ICurrencyStorageContract _currencyStorage;

    public ReportContract(IClientStorageContract clientStorage, IDepositStorageContract depositStorage, ICreditProgramStorageContract creditProgramStorage, ICurrencyStorageContract currencyStorage)
    {
        _clientStorage = clientStorage;
        _depositStorage = depositStorage;
        _currencyStorage = currencyStorage;
        _creditProgramStorage = creditProgramStorage;
    }
    public List<ReportClientCreditProgramViewModel> GetClientCreditProgram(List<ClientBindingModel> clients)
    {
        throw new NotImplementedException();
    }

    public List<ReportClientDepositViewModel> GetClientDeposit(ReportBindingModel model)
    {
        throw new NotImplementedException();
    }

    public List<ReportDepositCreditProgramViewModel> GetDepositCreditProgram(List<DepositBindingModel> deposits)
    {
        throw new NotImplementedException();
    }

    public List<ReportCreditProgramDepositCurrencyViewModel> GetDepositCurrency(ReportBindingModel model)
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveClientCreditProgramToExcel(ReportBindingModel model, List<ClientBindingModel> clients)
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveClientCreditProgramToWord(ReportBindingModel model, List<ClientBindingModel> clients)
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveClientDepositToPDF(ReportBindingModel model)
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveDepositCreditProgramToExcel(ReportBindingModel model, List<DepositBindingModel> currencies)
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveDepositCreditProgramToWord(ReportBindingModel model, List<DepositBindingModel> currencies)
    {
        throw new NotImplementedException();
    }

    public MemoryStream SaveDepositCurrencyToPDF(ReportBindingModel model)
    {
        throw new NotImplementedException();
    }
}
