using BankContracts.ViewModels;

namespace BankContracts.BindingModels;


public class DepositBindingModel
{
    public string? Id { get; set; }

    public float InterestRate { get; set; }

    public decimal Cost { get; set; }

    public int Period { get; set; }

    public string? ClerkId { get; set; }

    public List<DepositClientBindingModel>? DepositClients { get; set; }
}
