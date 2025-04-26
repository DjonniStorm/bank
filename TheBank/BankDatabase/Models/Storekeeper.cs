using BankContracts.DataModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Storekeeper
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required string MiddleName { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    [ForeignKey("StorekeeperId")]
    public List<Currency>? Currencies { get; set; }

    [ForeignKey("StorekeeperId")]
    public List<Period>? Periods { get; set; }

    [ForeignKey("StorekeeperId")]
    public List<CreditProgram>? CreditPrograms { get; set; }
}
