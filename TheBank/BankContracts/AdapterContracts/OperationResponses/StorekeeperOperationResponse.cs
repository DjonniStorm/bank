using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class StorekeeperOperationResponse : OperationResponse
{
    public static StorekeeperOperationResponse OK(List<StorekeeperViewModel> data) =>
        OK<StorekeeperOperationResponse, List<StorekeeperViewModel>>(data);

    public static StorekeeperOperationResponse OK(StorekeeperViewModel data) =>
        OK<StorekeeperOperationResponse, StorekeeperViewModel>(data);

    public static StorekeeperOperationResponse NoContent() => NoContent<StorekeeperOperationResponse>();

    public static StorekeeperOperationResponse NotFound(string message) =>
        NotFound<StorekeeperOperationResponse>(message);

    public static StorekeeperOperationResponse BadRequest(string message) =>
        BadRequest<StorekeeperOperationResponse>(message);

    public static StorekeeperOperationResponse InternalServerError(string message) =>
        InternalServerError<StorekeeperOperationResponse>(message);
}
