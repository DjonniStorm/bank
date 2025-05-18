namespace BankContracts.ViewModels;

public class ReportClientDepositViewModel
{
    public int DepositId { get; set; }
    public float InterestRate { get; set; }
    public int Period { get; set; }
    public List<ClientViewModel> Clients { get; set; } = new();
}
