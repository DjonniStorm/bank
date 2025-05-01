namespace BankContracts.ViewModels;

/// <summary>
/// модель представления для срока
/// </summary>
public class PeriodViewModel
{
    public required string Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public required string StorekeeperId { get; set; }
}
