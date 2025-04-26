using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Period
{
    public required string Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public required string StorekeeperId { get; set; }

    public Storekeeper? Storekeeper { get; set; }

    [ForeignKey("PeriodId")]
    public List<CreditProgram>? CreditPrograms { get; set; }
}
