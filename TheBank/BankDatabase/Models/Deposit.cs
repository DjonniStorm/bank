using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

public class Deposit
{
    public required string Id { get; set; }

    public float InterestRate { get; set; }

    public decimal Cost { get; set; }

    public int Period { get; set; }

    public required string ClerkId { get; set; }

    public Clerk? Clerk { get; set; }

    [ForeignKey("DepositId")]
    public List<DepositClient>? DepositClients { get; set; }

    [ForeignKey("DepositId")]
    public List<DepositCurrency>? DepositCurrencies { get; set; }

    [ForeignKey("DepositId")]
    public List<Replenishment>? Replenishments { get; set; }
}
