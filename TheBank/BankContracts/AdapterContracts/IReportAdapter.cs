using BankContracts.AdapterContracts.OperationResponses;

namespace BankContracts.AdapterContracts;

public interface IReportAdapter
{
    Task<ReportOperationResponse> GetDataClientsByCreditProgramAsync(CancellationToken ct);
    Task<ReportOperationResponse> GetDataClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<ReportOperationResponse> GetDataDepositByCreditProgramAsync(CancellationToken ct);
    Task<ReportOperationResponse> GetDataDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentClientsByCreditProgramAsync(CancellationToken ct);
    Task<ReportOperationResponse> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<ReportOperationResponse> CreateDocumentDepositByCreditProgramAsync(CancellationToken ct);
    Task<ReportOperationResponse> CreateDocumentDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
}
