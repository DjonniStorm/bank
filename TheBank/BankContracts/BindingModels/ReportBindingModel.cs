namespace BankContracts.BindingModels;

public class ReportBindingModel
{
    public string FileName { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
