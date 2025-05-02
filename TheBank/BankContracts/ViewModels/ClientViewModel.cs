namespace BankContracts.ViewModels;

public class ClientViewModel
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required decimal Balance { get; set; }

    public required string ClerkId { get; set; }

    public required List<DepositClientViewModel> DepositClients { get; set; }

    public required List<ClientCreditProgramViewModel>? CreditProgramClients { get; set; }
}
