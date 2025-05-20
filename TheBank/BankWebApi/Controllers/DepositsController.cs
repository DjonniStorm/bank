using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
[Produces("application/json")]
public class DepositsController(IDepositAdapter adapter) : ControllerBase
{
    private readonly IDepositAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей вкладов
    /// </summary>
    /// <returns>список вкладов</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о вкладе по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись вклада</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей вкладов по уникальному идентификатору клерка
    /// </summary>
    /// <param name="data">уникальный идентификатор клерка</param>
    /// <returns>список вкладов</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByClerk(string data)
    {
        return _adapter.GetListByClerk(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи вклада
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] DepositBindingModel model)
    {
        return _adapter.MakeDeposit(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи вклада
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] DepositBindingModel model)
    {
        return _adapter.ChangeDepositInfo(model).GetResponse(Request, Response);
    }
}
