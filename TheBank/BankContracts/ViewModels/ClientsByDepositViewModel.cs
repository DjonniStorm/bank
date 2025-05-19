namespace BankContracts.ViewModels;

public class ClientsByDepositViewModel
{
    public required string ClientSurname { get; set; }
    public required string ClientName { get; set; }
    public required decimal ClientBalance { get; set; }
    public required float DepositRate { get; set; }
    public required int DepositPeriod { get; set; }
    public DateTime FromPeriod { get; set; }
    public DateTime ToPeriod { get; set; }
}
