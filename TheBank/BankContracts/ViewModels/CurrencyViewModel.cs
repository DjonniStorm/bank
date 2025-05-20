namespace BankContracts.ViewModels;

public class CurrencyViewModel
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Abbreviation { get; set; }

    public required decimal Cost { get; set; }

    public required string StorekeeperId { get; set; }
}
