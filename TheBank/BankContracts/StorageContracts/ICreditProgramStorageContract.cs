using BankContracts.DataModels;

namespace BankContracts.StorageContracts;

public interface ICreditProgramStorageContract
{
    List<CreditProgramDataModel> GetList(string? storekeeperId = null, string? periodId = null);

    Task<List<CreditProgramDataModel>> GetListAsync(DateTime startDate, DateTime endDate, CancellationToken ct);

    CreditProgramDataModel? GetElementById(string id);

    void AddElement(CreditProgramDataModel creditProgramDataModel);

    void UpdElement(CreditProgramDataModel creditProgramDataModel);
}
