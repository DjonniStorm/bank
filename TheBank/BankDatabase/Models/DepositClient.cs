using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

public class DepositClient
{
    public required string DepositId { get; set; }

    public required string ClientId { get; set; }

    public Deposit? Deposit { get; set; }

    public Client? Client { get; set; }
}
