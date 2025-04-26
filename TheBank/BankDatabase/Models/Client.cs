using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Client
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public decimal Balance { get; set; }

    public required string ClerkId { get; set; }

    public Clerk? Clerk { get; set; }

    [ForeignKey("ClientId")]
    public List<ClientCreditProgram>? CreditProgramClients { get; set; }

    [ForeignKey("ClientId")]
    public List<DepositClient>? DepositClients { get; set; }
}
