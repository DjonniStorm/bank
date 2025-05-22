using BankBusinessLogic.Implementations;
using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class ReportController(IReportAdapter adapter) : ControllerBase
{
    private readonly IReportAdapter _adapter = adapter;
    private readonly EmailService _emailService = EmailService.CreateYandexService();

    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetClientByCreditProgram(CancellationToken ct)
    {
        return (await
        _adapter.GetDataClientsByCreditProgramAsync(ct)).GetResponse(Request, Response);
    }


    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadClientsByCreditProgram(CancellationToken cancellationToken)
    {
        return (await
        _adapter.CreateDocumentClientsByCreditProgramAsync(cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadExcelClientByCreditProgram(CancellationToken cancellationToken)
    {
        return (await
        _adapter.CreateExcelDocumentClientsByCreditProgramAsync(cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetClientByDeposit(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataClientsByDepositAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadClientsByDeposit(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentClientsByDepositAsync(fromDate,
        toDate, cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetDepositByCreditProgram(CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataDepositByCreditProgramAsync(cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadDepositByCreditProgram(CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentDepositByCreditProgramAsync(cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadExcelDepositByCreditProgram(CancellationToken cancellationToken)
    {
        return (await _adapter.CreateExcelDocumentDepositByCreditProgramAsync(cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetDepositAndCreditProgramByCurrency(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadDepositAndCreditProgramByCurrency(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    [HttpPost]
    public async Task<IActionResult> SendReportByCreditProgram([FromBody] MailSendInfoBindingModel model, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentClientsByCreditProgramAsync(ct);
            var stream = report.GetStream();
            var fileName = report.GetFileName() ?? "report.docx";
            if (stream == null)
                return BadRequest("Не удалось сформировать отчет");
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            await _emailService.SendReportAsync(
                toEmail: model.ToEmail,
                subject: model.Subject,
                body: model.Body,
                attachmentPath: tempPath
            );
            System.IO.File.Delete(tempPath);
            return Ok("Отчет успешно отправлен на почту");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendReportByDeposit([FromBody] MailSendInfoBindingModel model, DateTime fromDate, DateTime toDate, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentClientsByDepositAsync(fromDate, toDate, ct);
            var stream = report.GetStream();
            var fileName = report.GetFileName() ?? "report.pdf";
            if (stream == null)
                return BadRequest("Не удалось сформировать отчет");
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            await _emailService.SendReportAsync(
                toEmail: model.ToEmail,
                subject: model.Subject,
                body: model.Body,
                attachmentPath: tempPath
            );
            System.IO.File.Delete(tempPath);
            return Ok("Отчет успешно отправлен на почту");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendReportByCurrency([FromBody] MailSendInfoBindingModel model, DateTime fromDate, DateTime toDate, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, ct);
            var stream = report.GetStream();
            var fileName = report.GetFileName() ?? "report.pdf";
            if (stream == null)
                return BadRequest("Не удалось сформировать отчет");
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            await _emailService.SendReportAsync(
                toEmail: model.ToEmail,
                subject: model.Subject,
                body: model.Body,
                attachmentPath: tempPath
            );
            System.IO.File.Delete(tempPath);
            return Ok("Отчет успешно отправлен на почту");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendExcelReportByCreditProgram([FromBody] MailSendInfoBindingModel model, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateExcelDocumentClientsByCreditProgramAsync(ct);
            var stream = report.GetStream();
            var fileName = report.GetFileName() ?? "report.xlsx";
            if (stream == null)
                return BadRequest("Не удалось сформировать отчет");
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            await _emailService.SendReportAsync(
                toEmail: model.ToEmail,
                subject: model.Subject,
                body: model.Body,
                attachmentPath: tempPath
            );
            System.IO.File.Delete(tempPath);
            return Ok("Excel отчет успешно отправлен на почту");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendReportDepositByCreditProgram([FromBody] MailSendInfoBindingModel model, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentDepositByCreditProgramAsync(ct);
            var stream = report.GetStream();
            var fileName = report.GetFileName() ?? "report.docx";
            if (stream == null)
                return BadRequest("Не удалось сформировать отчет");
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            await _emailService.SendReportAsync(
                toEmail: model.ToEmail,
                subject: model.Subject,
                body: model.Body,
                attachmentPath: tempPath
            );
            System.IO.File.Delete(tempPath);
            return Ok("Отчет успешно отправлен на почту");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendExcelReportDepositByCreditProgram([FromBody] MailSendInfoBindingModel model, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateExcelDocumentDepositByCreditProgramAsync(ct);
            var stream = report.GetStream();
            var fileName = report.GetFileName() ?? "report.xlsx";
            if (stream == null)
                return BadRequest("Не удалось сформировать отчет");
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            await _emailService.SendReportAsync(
                toEmail: model.ToEmail,
                subject: model.Subject,
                body: model.Body,
                attachmentPath: tempPath
            );
            System.IO.File.Delete(tempPath);
            return Ok("Excel отчет успешно отправлен на почту");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }
}
