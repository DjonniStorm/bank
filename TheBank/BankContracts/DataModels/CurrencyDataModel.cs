using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель валюты
/// </summary>
/// <param name="id">уникальный Guid индентификатор</param>
/// <param name="name">название</param>
/// <param name="abbreviation">аббревиатура</param>
/// <param name="cost">стоимость валюты</param>
/// <param name="storekeeperId">уникальный Guid Индентификатор кладовщика</param>
public class CurrencyDataModel(string id, string name, string abbreviation, decimal cost, string storekeeperId) : IValidation
{
    public string Id { get; private set; } = id;

    public string Name { get; private set; } = name;

    public string Abbreviation { get; private set; } = abbreviation;

    public decimal Cost { get; private set; } = cost;

    public string StorekeeperId { get; private set; } = storekeeperId;

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
        if (Abbreviation.IsEmpty())
        {
            throw new ValidationException("Field Abbreviation is null or empty");
        }
        if (Cost <= 0)
        {
            throw new ValidationException("Field Cost is less or equal to zero");
        }
        if (StorekeeperId.IsEmpty())
        {
            throw new ValidationException("Field StorekeeperId is null or empty");
        }
        if (!StorekeeperId.IsGuid())
        {
            throw new ValidationException("The value in the field StorekeeperId is not a unique identifier");
        }
    }
}
