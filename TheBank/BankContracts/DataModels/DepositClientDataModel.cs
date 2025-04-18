using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель вклад клиент
/// </summary>
/// <param name="depositId">уникальный Guid индентификатор вклада</param>
/// <param name="clientId">уникальный Guid индентификатор клиента</param>
public class DepositClientDataModel(string depositId, string clientId) : IValidation
{
    public string DepositId { get; private set; } = depositId;

    public string ClientId { get; private set; } = clientId;

    public void Validate()
    {
        if (DepositId.IsEmpty())
        {
            throw new ValidationException("Field DepositId is null or empty");
        }
        if (!DepositId.IsGuid())
        {
            throw new ValidationException("The value in the field DepositId is not a unique identifier");
        }
        if (ClientId.IsEmpty())
        {
            throw new ValidationException("Field ClientId is null or empty");
        }
        if (!ClientId.IsGuid())
        {
            throw new ValidationException("The value in the field ClientId is not a unique identifier");
        }
    }
}
