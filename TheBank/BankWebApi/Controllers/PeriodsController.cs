using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
[Produces("application/json")]
public class PeriodsController(IPeriodAdapter adapter) : ControllerBase
{
    private readonly IPeriodAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей сроков
    /// </summary>
    /// <returns>список сроков</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о сроке по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись срока</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей сроков по уникальному идентификатору кладовщика
    /// </summary>
    /// <param name="data">уникальный идентификатор кладовщика</param>
    /// <returns>список сроков</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByStorekeeper(string data)
    {
        return _adapter.GetListByStorekeeper(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей сроков по дате начала
    /// </summary>
    /// <param name="data">дата начала</param>
    /// <returns>список сроков</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByStartTime(DateTime data)
    {
        return _adapter.GetListByStartTime(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей сроков по дате конца
    /// </summary>
    /// <param name="data">дата конца</param>
    /// <returns>список сроков</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByEndTime(DateTime data)
    {
        return _adapter.GetListByEndTime(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи срока
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] PeriodBindingModel model)
    {
        return _adapter.RegisterPeriod(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи срока
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] PeriodBindingModel model)
    {
        return _adapter.ChangePeriodInfo(model).GetResponse(Request, Response);
    }
}
