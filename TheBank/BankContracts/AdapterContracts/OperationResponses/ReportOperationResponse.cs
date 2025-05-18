namespace BankContracts.AdapterContracts.OperationResponses;

public class ReportOperationResponse
{
    Task<ReportOperationResponse> GetDepositsByClients(CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentDepositsByClients(CancellationToken ct);

    Task<ReportOperationResponse> GetClientsByDeposits(DateTime dateStart, DateTime dateFinish, CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentClientsByDeposits(DateTime dateStart, DateTime dateFinish, CancellationToken ct);

    Task<ReportOperationResponse> GetDepositsByCurrency(CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentDepositsByCurrency(CancellationToken ct);

    Task<ReportOperationResponse> GetCreditProgramByCurrency(DateTime dateStart, DateTime dateFinish, CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentClientsByDeposits(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
}
