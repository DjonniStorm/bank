using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
[Produces("application/json")]
public class CreditProgramsController(ICreditProgramAdapter adapter) : ControllerBase
{
    private readonly ICreditProgramAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей кредитных программ
    /// </summary>
    /// <returns>список кредитных программ</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о кредитной программе по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись кредитной программы</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей кредитных программ по уникальному идентификатору кладовщика
    /// </summary>
    /// <param name="data">уникальный идентификатор кладовщика</param>
    /// <returns>список кредитных программ</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByStorekeeper(string data)
    {
        return _adapter.GetListByStorekeeper(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// получение записей кредитных программ по уникальному идентификатору периода
    /// </summary>
    /// <param name="data">уникальный идентификатор периода</param>
    /// <returns>список кредитных программ</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecordByPeriod(string data)
    {
        return _adapter.GetListByPeriod(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи кредитной программы
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register([FromBody] CreditProgramBindingModel model)
    {
        return _adapter.RegisterCreditProgram(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи кредитной программы
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] CreditProgramBindingModel model)
    {
        return _adapter.ChangeCreditProgramInfo(model).GetResponse(Request, Response);
    }
}
