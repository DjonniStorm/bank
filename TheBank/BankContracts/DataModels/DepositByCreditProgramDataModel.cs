namespace BankContracts.DataModels;

public class DepositByCreditProgramDataModel
{
    public required string CreditProgramName { get; set; }
    public required List<float> DepositRate { get; set; }
    public required List<decimal> DepositCost { get; set; }
    public required List<int> DepositPeriod { get; set; }
}
