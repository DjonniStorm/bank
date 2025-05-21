using BankBusinessLogic.Implementations;
using BankContracts.AdapterContracts;
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
    public async Task<IActionResult> SendReportByCreditProgram(string email, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentClientsByCreditProgramAsync(ct);
            var response = report.GetResponse(Request, Response);
            
            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: email,
                    subject: "Отчет по клиентам по кредитным программам",
                    body: "<h1>Отчет по клиентам по кредитным программам</h1><p>В приложении находится отчет по клиентам по кредитным программам.</p>",
                    attachmentPath: tempPath
                );

                System.IO.File.Delete(tempPath);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendReportByDeposit(string email, DateTime fromDate, DateTime toDate, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentClientsByDepositAsync(fromDate, toDate, ct);
            var response = report.GetResponse(Request, Response);
            
            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: email,
                    subject: "Отчет по клиентам по вкладам",
                    body: $"<h1>Отчет по клиентам по вкладам</h1><p>Отчет за период с {fromDate:dd.MM.yyyy} по {toDate:dd.MM.yyyy}</p>",
                    attachmentPath: tempPath
                );

                System.IO.File.Delete(tempPath);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendReportByCurrency(string email, DateTime fromDate, DateTime toDate, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, ct);
            var response = report.GetResponse(Request, Response);
            
            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: email,
                    subject: "Отчет по вкладам и кредитным программам по валютам",
                    body: $"<h1>Отчет по вкладам и кредитным программам по валютам</h1><p>Отчет за период с {fromDate:dd.MM.yyyy} по {toDate:dd.MM.yyyy}</p>",
                    attachmentPath: tempPath
                );

                System.IO.File.Delete(tempPath);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendExcelReportByCreditProgram(string email, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateExcelDocumentClientsByCreditProgramAsync(ct);
            var response = report.GetResponse(Request, Response);
            
            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: email,
                    subject: "Excel отчет по клиентам по кредитным программам",
                    body: "<h1>Excel отчет по клиентам по кредитным программам</h1><p>В приложении находится Excel отчет по клиентам по кредитным программам.</p>",
                    attachmentPath: tempPath
                );

                System.IO.File.Delete(tempPath);
                return Ok("Excel отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendReportDepositByCreditProgram(string email, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentDepositByCreditProgramAsync(ct);
            var response = report.GetResponse(Request, Response);
            
            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: email,
                    subject: "Отчет по вкладам по кредитным программам",
                    body: "<h1>Отчет по вкладам по кредитным программам</h1><p>В приложении находится отчет по вкладам по кредитным программам.</p>",
                    attachmentPath: tempPath
                );

                System.IO.File.Delete(tempPath);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendExcelReportDepositByCreditProgram(string email, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateExcelDocumentDepositByCreditProgramAsync(ct);
            var response = report.GetResponse(Request, Response);
            
            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: email,
                    subject: "Excel отчет по вкладам по кредитным программам",
                    body: "<h1>Excel отчет по вкладам по кредитным программам</h1><p>В приложении находится Excel отчет по вкладам по кредитным программам.</p>",
                    attachmentPath: tempPath
                );

                System.IO.File.Delete(tempPath);
                return Ok("Excel отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }
}
