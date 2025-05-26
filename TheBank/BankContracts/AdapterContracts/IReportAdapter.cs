using BankContracts.AdapterContracts.OperationResponses;

namespace BankContracts.AdapterContracts;

public interface IReportAdapter
{
    Task<ReportOperationResponse> GetDataClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<ReportOperationResponse> GetDataClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<ReportOperationResponse> GetDataDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<ReportOperationResponse> GetDataDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<ReportOperationResponse> CreateExcelDocumentClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<ReportOperationResponse> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<ReportOperationResponse> CreateDocumentDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<ReportOperationResponse> CreateExcelDocumentDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<ReportOperationResponse> CreateDocumentDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
}
