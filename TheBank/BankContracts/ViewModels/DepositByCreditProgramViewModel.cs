namespace BankContracts.ViewModels;

public class DepositByCreditProgramViewModel
{
    public required string CreditProgramName { get; set; }
    public required List<float> DepositRate { get; set; }
    public required List<decimal> DepositCost { get; set; }
    public required List<int> DepositPeriod { get; set; }
}
