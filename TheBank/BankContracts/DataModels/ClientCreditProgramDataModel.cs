using BankContracts.Exceptions;
using BankContracts.Extensions;
using BankContracts.Infrastructure;

namespace BankContracts.DataModels;

/// <summary>
/// Дата модель клиент кредитная программа
/// </summary>
/// <param name="clientId"></param>
/// <param name="creditProgramId"></param>
public class ClientCreditProgramDataModel(string clientId, string creditProgramId) : IValidation
{
    public string ClientId { get; private set; } = clientId;

    public string CreditProgramId { get; private set; } = creditProgramId;

    public void Validate()
    {
        if (ClientId.IsEmpty())
        {
            throw new ValidationException("Field ClientId is null or empty");
        }
        if (!ClientId.IsGuid())
        {
            throw new ValidationException("The value in the field ClientId is not a unique identifier");
        }
        if (CreditProgramId.IsEmpty())
        {
            throw new ValidationException("Field CreditProgramId is null or empty");
        }
        if (!CreditProgramId.IsGuid())
        {
            throw new ValidationException("The value in the field CreditProgramId is not a unique identifier");
        }
    }
}
