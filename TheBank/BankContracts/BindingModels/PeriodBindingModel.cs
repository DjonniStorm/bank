namespace BankContracts.BindingModels;

/// <summary>
/// модель ответа от клиента для срока
/// </summary>
public class PeriodBindingModel
{
    public string? Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? StorekeeperId { get; set; }
}
