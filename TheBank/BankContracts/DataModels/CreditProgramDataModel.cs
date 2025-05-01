using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель кредитной программы
/// </summary>
/// <param name="id">уникальный Guid индентификатор</param>
/// <param name="name">название</param>
/// <param name="cost">сумма</param>
/// <param name="maxCost">максимальная сумма</param>
/// <param name="storekeeperId">уникальный Guid Индентификатор кладовщика</param>
/// <param name="periodId">уникальный Guid Индентификатор срока</param>
/// <param name="currencyCreditPrograms">валюты кредитной программы</param>
public class CreditProgramDataModel(string id, string name, decimal cost, decimal maxCost, string storekeeperId, string periodId,
    List<CreditProgramCurrencyDataModel> currencyCreditPrograms) : IValidation
{
    public string Id { get; private set; } = id;

    public string Name { get; private set; } = name;

    public decimal Cost { get; private set; } = cost;
    
    public decimal MaxCost { get; private set; } = maxCost;

    public string StorekeeperId { get; private set; } = storekeeperId;

    public string PeriodId { get; private set; } = periodId;

    public List<CreditProgramCurrencyDataModel> Currencies { get; private set; } = currencyCreditPrograms;

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
        if (Cost <= 0)
        {
            throw new ValidationException("Field Cost is less or equal to zero");
        }
        if (MaxCost <= 0)
        {
            throw new ValidationException("Field MaxCost is less or equal to zero");
        }
        if (StorekeeperId.IsEmpty())
        {
            throw new ValidationException("Field StorekeeperId is null or empty");
        }
        if (!StorekeeperId.IsGuid())
        {
            throw new ValidationException("The value in the field StorekeeperId is not a unique identifier");
        }
        if (PeriodId.IsEmpty())
        {
            throw new ValidationException("Field PeriodId is null or empty");
        }
        if (!PeriodId.IsGuid())
        {
            throw new ValidationException("The value in the field PeriodId is not a unique identifier");
        }
        if (Currencies is null)
        {
            throw new ValidationException("Field Currencies is null");
        }
    }
}
