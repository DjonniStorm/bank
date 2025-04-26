using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Currency
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Abbreviation { get; set; }

    public decimal Cost { get; set; }

    public required string StorekeeperId { get; set; }

    public Storekeeper? Storekeeper { get; set; }

    [ForeignKey("CurrencyId")]
    public List<CreditProgramCurrency>? CurrencyCreditPrograms { get; set; }

    [ForeignKey("CurrencyId")]
    public List<DepositCurrency>? DepositCurrencies { get; set; }
}
