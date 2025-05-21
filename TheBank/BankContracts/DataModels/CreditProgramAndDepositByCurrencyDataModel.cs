namespace BankContracts.DataModels;

public class CreditProgramAndDepositByCurrencyDataModel
{
    public required string CurrencyName { get; set; }
    public required List<string> CreditProgramName { get; set; }
    public required List<int> CreditProgramMaxCost { get; set; }
    public required List<float> DepositRate { get; set; }
    public required List<int> DepositPeriod { get; set; }
    public DateTime FromPeriod { get; set; }
    public DateTime ToPeriod { get; set; }
}
