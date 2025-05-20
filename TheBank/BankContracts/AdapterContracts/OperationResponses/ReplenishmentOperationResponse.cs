using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class ReplenishmentOperationResponse : OperationResponse
{
    public static ReplenishmentOperationResponse OK(List<ReplenishmentViewModel> data) =>
        OK<ReplenishmentOperationResponse, List<ReplenishmentViewModel>>(data);

    public static ReplenishmentOperationResponse OK(ReplenishmentViewModel data) =>
        OK<ReplenishmentOperationResponse, ReplenishmentViewModel>(data);

    public static ReplenishmentOperationResponse NoContent() => NoContent<ReplenishmentOperationResponse>();

    public static ReplenishmentOperationResponse NotFound(string message) =>
        NotFound<ReplenishmentOperationResponse>(message);

    public static ReplenishmentOperationResponse BadRequest(string message) =>
        BadRequest<ReplenishmentOperationResponse>(message);

    public static ReplenishmentOperationResponse InternalServerError(string message) =>
        InternalServerError<ReplenishmentOperationResponse>(message);
}
