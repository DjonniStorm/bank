using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class CreditProgram
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public decimal Cost { get; set; }

    public decimal MaxCost { get; set; }

    public required string StorekeeperId { get; set; }

    public required string PeriodId { get; set; }

    public Storekeeper? Storekeeper { get; set; }

    public Period? Period { get; set; }

    [ForeignKey("CreditProgramId")]
    public List<CreditProgramCurrency>? CurrencyCreditPrograms { get; set; }

    [ForeignKey("CreditProgramId")]
    public List<ClientCreditProgram>? CreditProgramClients { get; set; }
}
