namespace BankContracts.ViewModels;

/// <summary>
/// модель представления для клерка
/// </summary>
public class ClerkViewModel
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required string MiddleName { get; set; }

    public required string Login { get; set; }

    public required string Password { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }
}
