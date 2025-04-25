using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель клиента
/// </summary>
/// <param name="id">уникальный Guid индентификатор</param>
/// <param name="name">имя клиента</param>
/// <param name="surname">фамилия клиента</param>
/// <param name="balance">баланс клиента</param>
/// <param name="clerkId">уникальный Guid индентификатор клерка</param>
/// <param name="depositClients">вклады клиента</param>
/// <param name="creditProgramClients">кредитные программы клиента</param>
public class ClientDataModel(string id, string name, string surname, decimal balance, string clerkId,
    List<DepositClientDataModel> depositClients, List<ClientCreditProgramDataModel> creditProgramClients) : IValidation
{
    public string Id { get; private set; } = id;

    public string Name { get; private set; } = name;

    public string Surname { get; private set; } = surname;

    public decimal Balance { get; private set; } = balance;

    public string ClerkId { get; private set; } = clerkId;

    public List<DepositClientDataModel> Deposits { get; private set; } = depositClients;

    public List<ClientCreditProgramDataModel> CreditPrograms { get; private set; } = creditProgramClients;

    public void Validate()
    {
        if (Id.IsEmpty())
        {
            throw new ValidationException("Field Id is null or empty");
        }
        if (!Id.IsGuid())
        {
            throw new ValidationException("The value in the field Id is not a unique identifier");
        }
        if (Name.IsEmpty())
        {
            throw new ValidationException("Field Name is null or empty");
        }
        if (Surname.IsEmpty())
        {
            throw new ValidationException("Field Surname is null or empty");
        }
        if (Balance <= 0)
        {
            throw new ValidationException("Field Balance is less or equal to zero");
        }
        if (ClerkId.IsEmpty())
        {
            throw new ValidationException("Field Clerkid is null or empty");
        }
        if (!ClerkId.IsGuid())
        {
            throw new ValidationException("The value in the field Clerkid is not a unique identifier");
        }
        if ((Deposits?.Count ?? 0) == 0)
        {
            throw new ValidationException("The client must include deposits");
        }
        if ((CreditPrograms?.Count ?? 0) == 0)
        {
            throw new ValidationException("The client must include credit programs");
        }
    }
}
