using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class CurrencyOperationResponse : OperationResponse
{
    public static CurrencyOperationResponse OK(List<CurrencyViewModel> data) =>
        OK<CurrencyOperationResponse, List<CurrencyViewModel>>(data);

    public static CurrencyOperationResponse OK(CurrencyViewModel data) =>
        OK<CurrencyOperationResponse, CurrencyViewModel>(data);

    public static CurrencyOperationResponse NoContent() => NoContent<CurrencyOperationResponse>();

    public static CurrencyOperationResponse NotFound(string message) =>
        NotFound<CurrencyOperationResponse>(message);

    public static CurrencyOperationResponse BadRequest(string message) =>
        BadRequest<CurrencyOperationResponse>(message);

    public static CurrencyOperationResponse InternalServerError(string message) =>
        InternalServerError<CurrencyOperationResponse>(message);
}
