﻿namespace BankContracts.BindingModels;

public class MailSendInfoBindingModel
{
    public string ToEmail { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string? AttachmentPath { get; set; }
}
