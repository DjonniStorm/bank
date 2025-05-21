using BankContracts.DataModels;

namespace BankContracts.BusinessLogicContracts;

public interface IReportContract
{
    Task<List<ClientsByCreditProgramDataModel>> GetDataClientsByCreditProgramAsync(CancellationToken ct);
    Task<Stream> CreateDocumentClientsByCreditProgramAsync(CancellationToken ct);
    Task<Stream> CreateExcelDocumentClientsByCreditProgramAsync(CancellationToken ct);

    Task<List<ClientsByDepositDataModel>> GetDataClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<Stream> CreateDocumentClientsByDepositAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);

    Task<List<DepositByCreditProgramDataModel>> GetDataDepositByCreditProgramAsync(CancellationToken ct);
    Task<Stream> CreateDocumentDepositByCreditProgramAsync(CancellationToken ct);
    Task<Stream> CreateExcelDocumentDepositByCreditProgramAsync(CancellationToken ct);

    Task<List<CreditProgramAndDepositByCurrencyDataModel>> GetDataDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
    Task<Stream> CreateDocumentDepositAndCreditProgramByCurrencyAsync(DateTime dateStart, DateTime dateFinish, CancellationToken ct);
}
