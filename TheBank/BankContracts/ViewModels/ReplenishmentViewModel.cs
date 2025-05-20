namespace BankContracts.ViewModels;

public class ReplenishmentViewModel
{
    public required string Id { get; set; }

    public required decimal Amount { get; set; }

    public required DateTime Date { get; set; }

    public required string DepositId { get; set; }

    public required string ClerkId { get; set; }
}
