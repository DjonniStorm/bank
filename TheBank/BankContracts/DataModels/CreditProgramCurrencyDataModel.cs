using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель кредитная программа - валюта (многие ко многим)
/// </summary>
/// <param name="creditProgramId">уникальный Guid индентификатор кредитной программы</param>
/// <param name="currencyId">уникальный Guid индентификатор валюты</param>
public class CreditProgramCurrencyDataModel(string creditProgramId, string currencyId) : IValidation
{
    public string CreditProgramId { get; private set; } = creditProgramId;

    public string CurrencyId { get; private set; } = currencyId;

    public void Validate()
    {
        if (CreditProgramId.IsEmpty())
        {
            throw new ValidationException("Field CreditProgramId is null or empty");
        }
        if (!CreditProgramId.IsGuid())
        {
            throw new ValidationException("The value in the field CreditProgramId is not a unique identifier");
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
