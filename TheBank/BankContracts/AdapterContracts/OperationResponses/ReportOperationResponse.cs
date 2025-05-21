using BankContracts.Infrastructure;
using BankContracts.ViewModels;

namespace BankContracts.AdapterContracts.OperationResponses;

public class ReportOperationResponse : OperationResponse
{
    public static ReportOperationResponse OK(List<ClientsByCreditProgramViewModel> data) => OK<ReportOperationResponse, List<ClientsByCreditProgramViewModel>>(data);

    public static ReportOperationResponse OK(List<ClientsByDepositViewModel> data) => OK<ReportOperationResponse, List<ClientsByDepositViewModel>>(data);

    public static ReportOperationResponse OK(List<DepositByCreditProgramViewModel> data) => OK<ReportOperationResponse, List<DepositByCreditProgramViewModel>>(data);

    public static ReportOperationResponse OK(List<CreditProgramAndDepositByCurrencyViewModel> data) => OK<ReportOperationResponse, List<CreditProgramAndDepositByCurrencyViewModel>>(data);

    public static ReportOperationResponse OK(Stream data, string fileName) => OK<ReportOperationResponse, Stream>(data, fileName);

    public static ReportOperationResponse BadRequest(string message) => BadRequest<ReportOperationResponse>(message);

    public static ReportOperationResponse InternalServerError(string message) => InternalServerError<ReportOperationResponse>(message);

    public Stream? GetStream() => Result as Stream;
    public string? GetFileName() => FileName;
}
