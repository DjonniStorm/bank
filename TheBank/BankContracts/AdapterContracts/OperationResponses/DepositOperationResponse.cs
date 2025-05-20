using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class DepositOperationResponse : OperationResponse
{
    public static DepositOperationResponse OK(List<DepositViewModel> data) =>
        OK<DepositOperationResponse, List<DepositViewModel>>(data);

    public static DepositOperationResponse OK(DepositViewModel data) =>
        OK<DepositOperationResponse, DepositViewModel>(data);

    public static DepositOperationResponse NoContent() => NoContent<DepositOperationResponse>();

    public static DepositOperationResponse NotFound(string message) =>
        NotFound<DepositOperationResponse>(message);

    public static DepositOperationResponse BadRequest(string message) =>
        BadRequest<DepositOperationResponse>(message);

    public static DepositOperationResponse InternalServerError(string message) =>
        InternalServerError<DepositOperationResponse>(message);
}
