namespace BankContracts.ViewModels;

public class ReportClientCreditProgramViewModel
{
    public int CreditProgramId { get; set; }
    public string CreditProgramName { get; set; } = string.Empty;
    public List<ClientViewModel> Clients { get; set; } = new();
}
