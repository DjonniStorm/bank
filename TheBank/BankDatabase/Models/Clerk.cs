using System.ComponentModel.DataAnnotations.Schema;

namespace BankDatabase.Models;

class Clerk
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required string MiddleName { get; set; }

    public required string Login { get; set; }

    // тут хэш пароля
    public required string Password { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    [ForeignKey("ClerkId")]
    public List<Replenishment>? Replenishments { get; set; }

    [ForeignKey("ClerkId")]
    public List<Deposit>? Deposits { get; set; }

    [ForeignKey("ClerkId")]
    public List<Client>? Clients { get; set; }
}
