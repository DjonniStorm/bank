using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class PeriodOperationResponse : OperationResponse
{
    public static PeriodOperationResponse OK(List<PeriodViewModel> data) =>
        OK<PeriodOperationResponse, List<PeriodViewModel>>(data);

    public static PeriodOperationResponse OK(PeriodViewModel data) =>
        OK<PeriodOperationResponse, PeriodViewModel>(data);

    public static PeriodOperationResponse NoContent() => NoContent<PeriodOperationResponse>();

    public static PeriodOperationResponse NotFound(string message) =>
        NotFound<PeriodOperationResponse>(message);

    public static PeriodOperationResponse BadRequest(string message) =>
        BadRequest<PeriodOperationResponse>(message);

    public static PeriodOperationResponse InternalServerError(string message) =>
        InternalServerError<PeriodOperationResponse>(message);
}
