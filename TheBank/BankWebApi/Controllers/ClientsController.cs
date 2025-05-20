using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
[Produces("application/json")]
public class ClientsController(IClientAdapter adapter) : ControllerBase
{
    private readonly IClientAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей клиентов
    /// </summary>
    /// <returns>список клиентов</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о клиенте по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись клиента</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей клиента по уникальному идентификатору клерка
    /// </summary>
    /// <param name="data">уникальный идентификатор клерка</param>
    /// <returns>список клиентов</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByClerk(string data)
    {
        return _adapter.GetListByClerk(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи клиента
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] ClientBindingModel model)
    {
        return _adapter.RegisterClient(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи клиента 
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] ClientBindingModel model)
    {
        return _adapter.ChangeClientInfo(model).GetResponse(Request, Response);
    }
}
