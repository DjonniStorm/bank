using BankContracts.AdapterContracts;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(IReportAdapter reportAdapter) : ControllerBase
{
    private IReportAdapter _reportAdapter = reportAdapter;
}
