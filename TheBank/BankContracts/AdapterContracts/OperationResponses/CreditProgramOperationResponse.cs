using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class CreditProgramOperationResponse : OperationResponse
{
    public static CreditProgramOperationResponse OK(List<CreditProgramViewModel> data) =>
        OK<CreditProgramOperationResponse, List<CreditProgramViewModel>>(data);

    public static CreditProgramOperationResponse OK(CreditProgramViewModel data) =>
        OK<CreditProgramOperationResponse, CreditProgramViewModel>(data);

    public static CreditProgramOperationResponse NoContent() => NoContent<CreditProgramOperationResponse>();

    public static CreditProgramOperationResponse NotFound(string message) =>
        NotFound<CreditProgramOperationResponse>(message);

    public static CreditProgramOperationResponse BadRequest(string message) =>
        BadRequest<CreditProgramOperationResponse>(message);

    public static CreditProgramOperationResponse InternalServerError(string message) =>
        InternalServerError<CreditProgramOperationResponse>(message);
}
