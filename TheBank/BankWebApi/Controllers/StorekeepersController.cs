using BankContracts.AdapterContracts;
using BankContracts.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankWebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class StorekeepersController(IStorekeeperAdapter adapter) : ControllerBase
{
    private readonly IStorekeeperAdapter _adapter = adapter;

    /// <summary>
    /// получение всех записей кладовщика
    /// </summary>
    /// <returns>список кладовщиков</returns>
    [HttpGet]
    public IActionResult GetAllRecords()
    {
        return _adapter.GetList().GetResponse(Request, Response);
    }

    /// <summary>
    /// получние записи о кладовщике по данным
    /// </summary>
    /// <param name="data">уникальный идентификатор или другое поле</param>
    /// <returns>запись кладовщика</returns>
    [HttpGet("{data}")]
    public IActionResult GetRecord(string data)
    {
        return _adapter.GetElement(data).GetResponse(Request, Response);
    }

    /// <summary>
    /// создание записи кладовщика
    /// </summary>
    /// <param name="model">модель от пользователя</param>
    /// <returns></returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] StorekeeperBindingModel model)
    {
        return _adapter.RegisterStorekeeper(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// изменение записи кладовщика
    /// </summary>
    /// <param name="model">новая модель</param>
    /// <returns></returns>
    [HttpPut]
    public IActionResult ChangeInfo([FromBody] StorekeeperBindingModel model)
    {
        return _adapter.ChangeStorekeeperInfo(model).GetResponse(Request, Response);
    }

    /// <summary>
    /// вход для кладовщика
    /// </summary>
    /// <param name="model">модель с логином и паролем</param>
    /// <returns></returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] StorekeeperAuthBindingModel model) 
    {
        var res = _adapter.Login(model, out string token);

        Response.Cookies.Append(AuthOptions.CookieName, token);

        return res.GetResponse(Request, Response);
    }
}
