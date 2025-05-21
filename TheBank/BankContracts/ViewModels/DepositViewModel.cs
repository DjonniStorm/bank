using BankContracts.BindingModels;

namespace BankContracts.ViewModels;

/// <summary>
/// модель представления для вклада
/// </summary>
public class DepositViewModel
{
    public required string Id { get; set; }

    public required float InterestRate { get; set; }

    public required decimal Cost { get; set; }

    public required int Period { get; set; }

    public required string ClerkId { get; set; }

    public required List<DepositCurrencyBindingModel>? DepositCurrencies { get; set; }

}
