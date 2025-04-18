using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель пополнения
/// </summary>
/// <param name="id">уникальный Guid индентификатор</param>
/// <param name="amount">сумма пополнения</param>
/// <param name="date">дата пополнения</param>
/// <param name="depositId">уникальный Guid индентификатор вклада</param>
/// <param name="clerkId">уникальный Guid индентификатор клерка</param>
public class ReplenishmentDataModel(string id, decimal amount, DateTime date, string depositId, string clerkId) : IValidation
{
    public string Id { get; private set; } = id;

    public decimal Amount { get; private set; } = amount;

    public DateTime Date { get; private set; } = date;

    public string DepositId { get; private set; } = depositId;

    public string ClerkId { get; private set; } = clerkId;

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
        if (Amount <= 0)
        {
            throw new ValidationException("Field Amount is less or equal to zero");
        }
        if (DepositId.IsEmpty())
        {
            throw new ValidationException("Field DepositId is null or empty");
        }
        if (!DepositId.IsGuid())
        {
            throw new ValidationException("The value in the field DepositId is not a unique identifier");
        }
        if (ClerkId.IsEmpty())
        {
            throw new ValidationException("Field ClerkId is null or empty");
        }
        if (!ClerkId.IsGuid())
        {
            throw new ValidationException("The value in the field ClerkId is not a unique identifier");
        }
    }
}
