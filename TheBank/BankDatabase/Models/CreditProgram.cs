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
    //надо???
    [ForeignKey("StorekeeperId")]
    public Storekeeper? Storekeeper { get; set; }
    //и это надо??
    [ForeignKey("PeriodId")]
    public Period? Period { get; set; }

    [ForeignKey("CurrencyId")]
    public List<CreditProgram>? Currencies { get; set; }
}
