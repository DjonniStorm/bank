using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель вклада
/// </summary>
/// <param name="id">уникальный Guid индентификатор</param>
/// <param name="interestRate">процентная ставка</param>
/// <param name="cost">стоимость</param>
/// <param name="period">срок</param>
/// <param name="clerkId">уникальный Guid индентификатор клерка</param>
public class DepositDataModel(string id, float interestRate, decimal cost, int period, string clerkId) : IValidation
{
    public string Id { get; private set; } = id;

    public float InterestRate { get; private set; } = interestRate;

    public decimal Cost { get; private set; } = cost;

    public int Period { get; private set; } = period;

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
        if (InterestRate <= 0)
        {
            throw new ValidationException("Field InterestRate is less or equal to zero");
        }
        if (Cost <= 0)
        {
            throw new ValidationException("Field Cost is less or equal to zero");
        }
        if (Period <= 0)
        {
            throw new ValidationException("Field Period is less or equal to zero");
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
