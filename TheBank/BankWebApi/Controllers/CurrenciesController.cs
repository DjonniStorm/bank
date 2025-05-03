using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
[Produces("application/json")]
public class CurrenciesController(ICurrencyAdapter adapter) : ControllerBase
{
    private readonly ICurrencyAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей валют
    /// </summary>
    /// <returns>список валют</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о валюте по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись вклада</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей валюте по уникальному идентификатору кладовщика
    /// </summary>
    /// <param name="data">уникальный идентификатор кладовщика</param>
    /// <returns>список валют</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByStorekeeper(string data)
    {
        return _adapter.GetListByStorekeeper(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи валюты
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] CurrencyBindingModel model)
    {
        return _adapter.MakeCurrency(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи валюты
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] CurrencyBindingModel model)
    {
        return _adapter.ChangeCurrencyInfo(model).GetResponse(Request, Response);
    }
}
