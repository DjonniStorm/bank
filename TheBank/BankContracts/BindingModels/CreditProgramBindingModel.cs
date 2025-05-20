namespace BankContracts.BindingModels;

public class CreditProgramBindingModel
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public decimal Cost { get; set; }

    public decimal MaxCost { get; set; }

    public required string StorekeeperId { get; set; }

    public required string PeriodId { get; set; }

    public List<CreditProgramCurrencyBindingModel>? CurrencyCreditPrograms { get; set; }
}
