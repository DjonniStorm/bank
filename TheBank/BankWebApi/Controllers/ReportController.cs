using BankContracts.AdapterContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class ReportController(IReportAdapter adapter) : ControllerBase
{
    private IReportAdapter _adapter = adapter;

    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetClientByCreditProgram(CancellationToken ct)
    {
        return (await
        _adapter.GetDataClientsByCreditProgramAsync(ct)).GetResponse(Request, Response);
    }


    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadClientByCreditProgram(CancellationToken cancellationToken)
    {
        return (await
        _adapter.CreateDocumentClientsByCreditProgramAsync(cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/json")]
    public async Task<IActionResult> GetClientByDeposit(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataClientsByDepositAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadClientByDeposit(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
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
    [Consumes("application/json")]
    public async Task<IActionResult> GetDepositAndCreditProgramByCurrency(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.GetDataDepositAndCreditProgramByCurrencyAsync(fromDate, toDate,
        cancellationToken)).GetResponse(Request, Response);
    }

    [HttpGet]
    [Consumes("application/octet-stream")]
    public async Task<IActionResult> LoadDepositAndCreditProgramByCurrency(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        return (await _adapter.CreateDocumentDepositAndCreditProgramByCurrencyAsync(fromDate, toDate, cancellationToken)).GetResponse(Request, Response);
    }
}
