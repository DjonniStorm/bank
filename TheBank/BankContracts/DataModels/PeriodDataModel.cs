using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель срока
/// </summary>
/// <param name="id">уникальный Guid индентификатор</param>
/// <param name="startDate"></param>
/// <param name="endDate"></param>
/// <param name="storekeeperId">уникальный Guid индентификатор кладовщика</param>
public class PeriodDataModel(string id, DateTime startDate, DateTime endDate, string storekeeperId) : IValidation
{
    public string Id { get; private set; } = id;

    public DateTime StartTime { get; private set; } = startDate;

    public DateTime EndTime { get; private set; } = endDate;

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
        if (StartTime.Date > EndTime.Date)
        {
            throw new ValidationException($"he date of period start cannot be larger than the date of period end");
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
