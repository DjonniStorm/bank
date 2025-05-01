using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class ClerkOperationResponse : OperationResponse
{
    public static ClerkOperationResponse OK(List<ClerkViewModel> data) =>
        OK<ClerkOperationResponse, List<ClerkViewModel>>(data);

    public static ClerkOperationResponse OK(ClerkViewModel data) =>
        OK<ClerkOperationResponse, ClerkViewModel>(data);

    public static ClerkOperationResponse NoContent() => NoContent<ClerkOperationResponse>();

    public static ClerkOperationResponse NotFound(string message) =>
        NotFound<ClerkOperationResponse>(message);

    public static ClerkOperationResponse BadRequest(string message) =>
        BadRequest<ClerkOperationResponse>(message);

    public static ClerkOperationResponse InternalServerError(string message) =>
        InternalServerError<ClerkOperationResponse>(message);
}
