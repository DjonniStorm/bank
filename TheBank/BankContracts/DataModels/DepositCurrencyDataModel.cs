using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель вклад валюта
/// </summary>
/// <param name="depositId">уникальный Guid индентификатор вклада</param>
/// <param name="currencyId">уникальный Guid индентификатор валюты</param>
public class DepositCurrencyDataModel(string depositId, string currencyId) : IValidation
{
    public string DepositId { get; private set; } = depositId;

    public string CurrencyId { get; private set; } = currencyId;

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
        if (CurrencyId.IsEmpty())
        {
            throw new ValidationException("Field CurrencyId is null or empty");
        }
        if (!CurrencyId.IsGuid())
        {
            throw new ValidationException("The value in the field CurrencyId is not a unique identifier");
        }
    }
}
