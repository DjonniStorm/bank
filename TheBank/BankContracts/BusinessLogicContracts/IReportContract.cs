using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IReportContract
{
    Task<List<ClientsByCreditProgramDataModel>> GetDataClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<Stream> CreateDocumentClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<Stream> CreateExcelDocumentClientsByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);

    Task<List<ClientsByDepositDataModel>> GetDataClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<Stream> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);

    Task<List<DepositByCreditProgramDataModel>> GetDataDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<Stream> CreateDocumentDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);
    Task<Stream> CreateExcelDocumentDepositByCreditProgramAsync(List<string>? creditProgramIds, CancellationToken ct);

    Task<List<CreditProgramAndDepositByCurrencyDataModel>> GetDataDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<Stream> CreateDocumentDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
}
