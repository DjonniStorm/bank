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

    /// <summary>
    /// получение всех записей клерков
    /// </summary>
    /// <returns>список клерков</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о клерке по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись клерка</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи клерка
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] ClerkBindingModel model)
    {
        return _adapter.RegisterClerk(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи клерка
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] ClerkBindingModel model)
    {
        return _adapter.ChangeClerkInfo(model).GetResponse(Request, Response);
    }
}
