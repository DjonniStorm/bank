namespace BankDatabase.Models;

class CreditProgramCurrency
{
    public required string CreditProgramId { get; set; }

    public required string CurrencyId { get; set; }

    public CreditProgram? CreditProgram { get; set; }

    public Currency? Currency { get; set; }
}
