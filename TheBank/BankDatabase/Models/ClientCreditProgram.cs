namespace BankDatabase.Models;

class ClientCreditProgram
{
    public required string ClientId { get; set; }

    public required string CreditProgramId { get; set; }

    public Client? Client { get; set; }

    public CreditProgram? CreditProgram { get; set; }
}
