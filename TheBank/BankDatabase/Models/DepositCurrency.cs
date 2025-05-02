namespace BankDatabase.Models;

class DepositCurrency
{
    public required string DepositId { get; set; }

    public required string CurrencyId { get; set; }

    public Deposit? Deposit { get; set; }

    public Currency? Currency { get; set; }
}
