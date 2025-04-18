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
public class ClientDataModel(string id, string name, string surname, decimal balance, string clerkId) : IValidation
{
    public string Id { get; private set; } = id;

    public string Name { get; private set; } = name;

    public string Surname { get; private set; } = surname;

    public decimal Balance { get; private set; } = balance;

    public string Clerkid { get; private set; } = clerkId;

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
        if (Clerkid.IsEmpty())
        {
            throw new ValidationException("Field Clerkid is null or empty");
        }
        if (!Clerkid.IsGuid())
        {
            throw new ValidationException("The value in the field Clerkid is not a unique identifier");
        }
    }
}
