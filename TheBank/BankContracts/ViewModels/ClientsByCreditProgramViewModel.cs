namespace BankContracts.ViewModels;

public class ClientsByCreditProgramViewModel
{
    public required string CreditProgramName { get; set; }
    public required List<string> ClientSurname { get; set; }
    public required List<string> ClientName { get; set; }
    public required List<decimal> ClientBalance { get; set; }
}
