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

    /// <summary>
    /// Получение данных Клиента по Кредитным программам
    /// </summary>
    /// <param name="creditProgramIds"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetClientByCreditProgram([FromQuery] List<string>? creditProgramIds, CancellationToken ct)
    {
        return (await _adapter.GetDataClientsByCreditProgramAsync(creditProgramIds, ct)).GetResponse(Request, Response);
    }
    /// <summary>
    /// Отчет word Клиента по Кредитным программам
    /// </summary>
    /// <param name="creditProgramIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadClientsByCreditProgram([FromQuery] List<string>? creditProgramIds, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentClientsByCreditProgramAsync(creditProgramIds, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Отчет excel Клиента по Кредитным программам
    /// </summary>
    /// <param name="creditProgramIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadExcelClientByCreditProgram([FromQuery] List<string>? creditProgramIds, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateExcelDocumentClientsByCreditProgramAsync(creditProgramIds, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Получение данных Клиента по Вкладам
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetClientByDeposit(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataClientsByDepositAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Отчет word Клиента по Вкладам
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadClientsByDeposit(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentClientsByDepositAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Получение данных Вклада по Кредитным программам
    /// </summary>
    /// <param name="creditProgramIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetDepositByCreditProgram([FromQuery] List<string>? creditProgramIds, CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataDepositByCreditProgramAsync(creditProgramIds, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Отчет word Вклада по Кредитным программам
    /// </summary>
    /// <param name="creditProgramIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadDepositByCreditProgram([FromQuery] List<string>? creditProgramIds, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentDepositByCreditProgramAsync(creditProgramIds, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Отчет excel Вклада по Кредитным программам
    /// </summary>
    /// <param name="creditProgramIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadExcelDepositByCreditProgram([FromQuery] List<string>? creditProgramIds, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateExcelDocumentDepositByCreditProgramAsync(creditProgramIds, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Получение данных Вклада и Кредитных программам по Валютам
    /// кладовщик pdf
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetDepositAndCreditProgramByCurrency(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Отчет pdf Вклада и Кредитных программам по Валютам
    /// кладовщик pdf
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadDepositAndCreditProgramByCurrency(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    /// <summary>
    /// Отправка word отчета Клиентов по Кредитным программам
    /// </summary>
    /// <param name="mailInfo"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SendReportByCreditProgram([FromBody] CreditProgramReportMailSendInfoBindingModel mailInfo, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentClientsByCreditProgramAsync(mailInfo.CreditProgramIds, ct);
            var response = report.GetResponse(Request, Response);

            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                var tempPathWithExtension = Path.ChangeExtension(tempPath, ".docx");

                using (var fileStream = new FileStream(tempPathWithExtension, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: mailInfo.Email,
                    subject: mailInfo.Subject,
                    body: mailInfo.Body,
                    attachmentPath: tempPathWithExtension
                );

                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(tempPathWithExtension);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    /// <summary>
    /// Отправка pdf отчета Клиентов по Депозитам
    /// </summary>
    /// <param name="mailInfo"></param>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SendReportByDeposit([FromBody] DepositReportMailSendInfoBindingModel mailInfo, DateTime fromDate, DateTime toDate, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentClientsByDepositAsync(fromDate, toDate, ct);
            var response = report.GetResponse(Request, Response);

            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                var tempPathWithExtension = Path.ChangeExtension(tempPath, ".pdf");

                using (var fileStream = new FileStream(tempPathWithExtension, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: mailInfo.Email,
                    subject: mailInfo.Subject,
                    body: mailInfo.Body,
                    attachmentPath: tempPathWithExtension
                );

                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(tempPathWithExtension);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    /// <summary>
    /// Отправка pdf отчета Вкладов и Кредитных программ по Валютам
    /// кладовщик pdf
    /// </summary>
    /// <param name="mailInfo"></param>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SendReportByCurrency([FromBody] DepositReportMailSendInfoBindingModel mailInfo, DateTime fromDate, DateTime toDate, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, ct);
            var response = report.GetResponse(Request, Response);

            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                var tempPathWithExtension = Path.ChangeExtension(tempPath, ".pdf");

                using (var fileStream = new FileStream(tempPathWithExtension, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: mailInfo.Email,
                    subject: mailInfo.Subject,
                    body: mailInfo.Body,
                    attachmentPath: tempPathWithExtension
                );

                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(tempPathWithExtension);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    /// <summary>
    /// Отправка excel отчета Клиентов по Кредитным программам 
    /// </summary>
    /// <param name="mailInfo"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SendExcelReportByCreditProgram([FromBody] CreditProgramReportMailSendInfoBindingModel mailInfo, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateExcelDocumentClientsByCreditProgramAsync(mailInfo.CreditProgramIds, ct);
            var response = report.GetResponse(Request, Response);

            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                var tempPathWithExtension = Path.ChangeExtension(tempPath, ".xlsx");

                using (var fileStream = new FileStream(tempPathWithExtension, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: mailInfo.Email,
                    subject: mailInfo.Subject,
                    body: mailInfo.Body,
                    attachmentPath: tempPathWithExtension
                );

                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(tempPathWithExtension);
                return Ok("Excel отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    /// <summary>
    /// Отправка word отчета Вкладов по Кредитных программ 
    /// </summary>
    /// <param name="mailInfo"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SendReportDepositByCreditProgram([FromBody] CreditProgramReportMailSendInfoBindingModel mailInfo, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateDocumentDepositByCreditProgramAsync(mailInfo.CreditProgramIds, ct);
            var response = report.GetResponse(Request, Response);

            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                var tempPathWithExtension = Path.ChangeExtension(tempPath, ".docx");

                using (var fileStream = new FileStream(tempPathWithExtension, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: mailInfo.Email,
                    subject: mailInfo.Subject,
                    body: mailInfo.Body,
                    attachmentPath: tempPathWithExtension
                );

                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(tempPathWithExtension);
                return Ok("Отчет успешно отправлен на почту");
            }

            return BadRequest("Не удалось получить отчет");
        }
        catch (Exception ex)
        {
            return BadRequest($"Ошибка при отправке отчета: {ex.Message}");
        }
    }

    /// <summary>
    /// Отправка excel отчета Вкладов по Кредитных программ
    /// </summary>
    /// <param name="mailInfo"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SendExcelReportDepositByCreditProgram([FromBody] CreditProgramReportMailSendInfoBindingModel mailInfo, CancellationToken ct)
    {
        try
        {
            var report = await _adapter.CreateExcelDocumentDepositByCreditProgramAsync(mailInfo.CreditProgramIds, ct);
            var response = report.GetResponse(Request, Response);

            if (response is FileStreamResult fileResult)
            {
                var tempPath = Path.GetTempFileName();
                var tempPathWithExtension = Path.ChangeExtension(tempPath, ".xlsx");

                using (var fileStream = new FileStream(tempPathWithExtension, FileMode.Create))
                {
                    await fileResult.FileStream.CopyToAsync(fileStream);
                }

                await _emailService.SendReportAsync(
                    toEmail: mailInfo.Email,
                    subject: mailInfo.Subject,
                    body: mailInfo.Body,
                    attachmentPath: tempPathWithExtension
                );

                System.IO.File.Delete(tempPath);
                System.IO.File.Delete(tempPathWithExtension);
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
