using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Replenishment
{
    public required string Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public required string DepositId { get; set; }

    public required string ClerkId { get; set; }

    [ForeignKey("ClerkId")]
    public Clerk? Clerk { get; set; }
}
