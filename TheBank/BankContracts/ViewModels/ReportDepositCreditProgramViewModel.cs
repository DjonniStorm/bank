namespace BankContracts.ViewModels;

public class ReportDepositCreditProgramViewModel
{
    public int CreditProgramId { get; set; }
    public string CreditProgramName { get; set; } = string.Empty;
    public List<DepositViewModel> Deposits { get; set; } = new();
}
