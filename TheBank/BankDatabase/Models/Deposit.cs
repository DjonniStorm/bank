using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Deposit
{
    public required string Id { get; set; }

    public float InterestRate { get; set; }

    public decimal Cost { get; set; }

    public int Period { get; set; }

    public required string ClerkId { get; set; }

    [ForeignKey("ClerkId")]
    public Clerk? Clerk { get; set; }
}
