using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Client
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public decimal Balance { get; set; }

    public required string ClerkId { get; set; }

    [ForeignKey("ClerkId")]
    public Clerk? Clerk { get; set; }

    [ForeignKey("DepositId")]
    public List<DepositClient>? Deposits { get; set; }

    [ForeignKey("CreditProgramId")]
    public List<ClientCreditProgram>? CreditPrograms { get; set; }
}
