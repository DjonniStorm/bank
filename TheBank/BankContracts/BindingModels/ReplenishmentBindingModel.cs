namespace BankContracts.BindingModels;

public class ReplenishmentBindingModel
{
    public string? Id { get; set; } 

    public decimal Amount { get; set; } 

    public DateTime Date { get; set; }

    public string? DepositId { get;  set; } 

    public string? ClerkId { get; set; } 
}
