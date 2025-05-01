using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ClerksController(IClerkAdapter adapter) : ControllerBase
{
    private readonly IClerkAdapter _adapter = adapter;

    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    [HttpPost]
    public IActionResult Register([FromBody] ClerkBindingModel model)
    {
        return _adapter.RegisterClerk(model).GetResponse(Request, Response);
    }

    [HttpPut]
    public IActionResult ChangeInfo([FromBody] ClerkBindingModel model)
    {
        return _adapter.ChangeClerkInfo(model).GetResponse(Request, Response);
    }
}
