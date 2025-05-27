namespace BankContracts.BindingModels;

public class ReportMailSendInfoBindingModel
{
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class CreditProgramReportMailSendInfoBindingModel : ReportMailSendInfoBindingModel
{
    public List<string> CreditProgramIds { get; set; } = new();
}

public class DepositReportMailSendInfoBindingModel : ReportMailSendInfoBindingModel
{
    // Для отчетов по депозитам дополнительные поля передаются через query параметры
}