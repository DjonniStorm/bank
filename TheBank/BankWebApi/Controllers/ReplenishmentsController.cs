using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
[Produces("application/json")]
public class ReplenishmentsController(IReplenishmentAdapter adapter) : ControllerBase
{
    private readonly IReplenishmentAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей пополнений
    /// </summary>
    /// <returns>список пополнений</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о пополнени по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись пополнения</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей пополнений по уникальному идентификатору клерка
    /// </summary>
    /// <param name="data">уникальный идентификатор клерка</param>
    /// <returns>список пополнений</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByClerk(string data)
    {
        return _adapter.GetListByClerk(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей пополнений по уникальному идентификатору вклада
    /// </summary>
    /// <param name="data">уникальный идентификатор вклада</param>
    /// <returns>список пополнений</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByDeposit(string data)
    {
        return _adapter.GetListByDeposit(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей пополнений по дате
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns>список пополнений</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByDate(DateTime fromDate, DateTime toDate)
    {
        return _adapter.GetListByDate(fromDate, toDate).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи пополнения
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] ReplenishmentBindingModel model)
    {
        return _adapter.RegisterReplenishment(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи пополнения
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] ReplenishmentBindingModel model)
    {
        return _adapter.ChangeReplenishmentInfo(model).GetResponse(Request, Response);
    }
}
