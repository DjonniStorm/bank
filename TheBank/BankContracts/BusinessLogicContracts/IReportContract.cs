using BankContracts.BindingModels;
using BankContracts.ViewModels;

namespace BankContracts.BusinessLogicContracts;

public interface IReportContract
{
    List<ReportClientCreditProgramViewModel> GetClientCreditProgram(List<ClientBindingModel> clients);
    List<ReportClientDepositViewModel> GetClientDeposit(ReportBindingModel model);
    MemoryStream SaveClientCreditProgramToWord(ReportBindingModel model, List<ClientBindingModel> clients);
    MemoryStream SaveClientCreditProgramToExcel(ReportBindingModel model, List<ClientBindingModel> clients);
    MemoryStream SaveClientDepositToPDF(ReportBindingModel model);

    List<ReportDepositCreditProgramViewModel> GetDepositCreditProgram(List<DepositBindingModel> deposits);
    List<ReportCreditProgramDepositCurrencyViewModel> GetDepositCurrency(ReportBindingModel model);
    MemoryStream SaveDepositCreditProgramToWord(ReportBindingModel model, List<DepositBindingModel> currencies);
    MemoryStream SaveDepositCreditProgramToExcel(ReportBindingModel model, List<DepositBindingModel> currencies);
    MemoryStream SaveDepositCurrencyToPDF(ReportBindingModel model);
}
