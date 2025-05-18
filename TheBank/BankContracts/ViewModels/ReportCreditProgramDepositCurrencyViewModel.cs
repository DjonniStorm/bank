namespace BankContracts.ViewModels;

public class ReportCreditProgramDepositCurrencyViewModel
{
    public int CurrencyId { get; set; }
    public string CreditProgramName { get; set; } = string.Empty;
    public decimal CreditProgramMaxCost { get; set; }
    public List<DepositViewModel> Deposits { get; set; } = new();
}
