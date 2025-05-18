namespace BankContracts.AdapterContracts.OperationResponses;

public class ReportOperationResponse
{
    Task<ReportOperationResponse> GetCreditProgramByDeposits(CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentCreditProgramByDeposits(CancellationToken ct);

    Task<ReportOperationResponse> GetClientsByDeposits(CancellationToken ct);

    Task<ReportOperationResponse> CreateDocumentClientsByDeposits(CancellationToken ct);
}
