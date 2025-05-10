namespace BankContracts.BindingModels;

public class ClientBindingModel
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public decimal Balance { get; set; }

    public string? ClerkId { get; set; }

    public List<DepositClientBindingModel>? DepositClients { get; set; }

    public List<ClientCreditProgramBindingModel>? CreditProgramClients { get; set; }
}
