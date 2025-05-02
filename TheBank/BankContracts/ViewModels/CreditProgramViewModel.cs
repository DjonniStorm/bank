namespace BankContracts.ViewModels;

/// <summary>
/// модель представления для кредитной программы
/// </summary>
public class CreditProgramViewModel
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public decimal Cost { get; set; }

    public decimal MaxCost { get; set; }

    public required string StorekeeperId { get; set; }

    public required string PeriodId { get; set; }

    public required List<CreditProgramCurrencyViewModel>? CurrencyCreditPrograms { get; set; }
}
