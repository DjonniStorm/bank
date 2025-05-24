namespace BankContracts.DataModels;

public class ClientsByCreditProgramDataModel
{
    public required string CreditProgramId { get; set; }
    public required string CreditProgramName { get; set; }
    public required List<string> ClientSurname { get; set; }
    public required List<string> ClientName { get; set; }
    public required List<decimal> ClientBalance { get; set; }
}
