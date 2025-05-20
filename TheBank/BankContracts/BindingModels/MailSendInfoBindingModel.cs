namespace BankContracts.BindingModels;

public class MailSendInfoBindingModel
{
    public string MailAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public MemoryStream Attachment { get; set; } = new MemoryStream();
    public string FileName { get; set; } = string.Empty;
}
